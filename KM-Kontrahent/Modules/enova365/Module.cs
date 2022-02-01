using KM_Kontrahent.Managers;
using KM_Kontrahent.Models;
using KM_Kontrahent.Modules.enova365;
using itDesk.Business.Debuggers.Enums;
using itDesk.Xml.Serialization;
using Soneta.Business;
using Soneta.Business.App;
using Soneta.Core;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Kasa;
using Soneta.Magazyny;
using Soneta.Towary;
using Soneta.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Login = Soneta.Business.App.Login;

namespace KM_Kontrahent.Modules.enova365
{
    public class Module : IDisposable
    {
        #region -- CONSTANTS --

        #endregion

        #region -- FIELDS --

        private Login _login;
        public String DirectoryPath { get; }

        #endregion

        #region -- METHODS --
        private static String CalculateHash(String input)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            Byte[] hash;

            using (var md5 = MD5.Create())
                hash = md5.ComputeHash(inputBytes);

            var builder = new StringBuilder();

            foreach (Byte hashByte in hash)
                builder.Append(hashByte.ToString("x2"));

            return builder.ToString();
        }

        public Boolean Login()
        {
            try
            {
                Database database = BusApplication.Instance[KMKontrahent.SyncManager.SettingsManager.Settings.Database];

                database.Active = true;

                _login = database.Login(false, KMKontrahent.SyncManager.SettingsManager.Settings.User.Name, KMKontrahent.SyncManager.SettingsManager.Settings.User.Password) ??
                         throw new InvalidOperationException("Nie udało się zalogować do bazy danych systemu enova365.");

                KMKontrahent.Logger.Log("Pomyślnie zalogowano do bazy danych systemu enova365.", LogType.Information);

                return true;
            }
            catch (Exception exception)
            {
                _login = null;

                KMKontrahent.Logger.Log("Wystąpił błąd podczas logowania do bazy danych systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return false;
            }
        }
        public void Logout()
        {
            _login?.Dispose();
            _login = null;

            KMKontrahent.Logger.Log("Pomyślnie wylogowano się z bazy danych systemu enova365.", LogType.Information);
        }

        public void Dispose() => Logout();


        public List<Contractor> GetContractor()
        {
            try
            {
                List<Contractor> contractors = new List<Contractor>();
                var dokumentyHandlowe = new List<DokumentHandlowy>();

                using (Session session = _login.CreateSession(true, false, "GetContractors"))
                {
                    var crmModule = CRMModule.GetInstance(session);

                    int countOfKontrahents = crmModule.Kontrahenci.WgKodu.Count();

                    var list = crmModule.Kontrahenci.WgKodu;

                    foreach (var item in list)
                    {
                        Contractor contractor = new Contractor();

                        contractor.Adres = item.Adres.ToString();
                        contractor.NIP = item.NIP;
                        contractor.Nazwa = item.Nazwa;
                        contractor.Email = item.EMAIL;
                        contractor.KodKraju = item.KodKraju;
                        contractor.Id = item.Kod;
                        contractor.PESEL = item.PESEL;

                        contractors.Add(contractor);

                        KMKontrahent.Logger.Log("Pomyślnie wyeksportowano dane kontrahenta o id " + contractor.Id, LogType.Information);

                    }


                }
                return contractors;
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas pobierania informacji o płatnościach z systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return new List<Contractor>();
            }
        }

        public List<Product> GetProducts()
        {
            try
            {
                List<Product> products = new List<Product>();
                var dokumentyHandlowe = new List<DokumentHandlowy>();

                using (Session session = _login.CreateSession(true, false, "Getproducts"))
                {
                    var handelModule = HandelModule.GetInstance(session);

                    int countOfKontrahents = handelModule.Towary.Towary.WgKodu.Count();

                    var list = handelModule.Towary.Towary.WgKodu;

                    foreach (var item in list)
                    {
                        Product product = new Product();

                        product.Id = item.ID.ToString();
                        product.Guid = item.Guid.ToString();
                        product.Kod = item.Kod;
                        product.Nazwa = item.Nazwa;
                        product.EAN = item.EAN;
                        product.DefinicjaStawki = item.DefinicjaStawki.ToString();

                        products.Add(product);
                        KMKontrahent.Logger.Log("Pomyślnie wyeksportowano dane towaru o id " + product.Id, LogType.Information);

                    }


                }
                return products;
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas pobierania informacji o płatnościach z systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return new List<Product>();
            }
        }

        public List<Result> ImportContractors(List<Contractor> contractors)
        {
            try
            {
                var results = new List<Result>();

                foreach (Contractor contractor in contractors)
                {
                    using (Session session = _login.CreateSession(false, false, "ImportContractors"))
                    {
                        var handelModule = HandelModule.GetInstance(session);
                        var crmModule = CRMModule.GetInstance(session);
                        var result = new Result
                        {
                            Record = contractor,
                            Success = true
                        };
                        Kontrahent kontrahent = null;
                        try
                        {
                            kontrahent = crmModule.Kontrahenci.WgKodu[contractor.Id];
                        }
                        catch (Exception exception)
                        {
                        }

                        using (ITransaction transaction = session.Logout(true))
                        {
                            if (kontrahent == null)
                            {
                                kontrahent = new Kontrahent();

                                crmModule.Kontrahenci.AddRow(kontrahent);

                            }

                            if (kontrahent.Nazwa == "!INCYDENTALNY")
                                continue;

                            kontrahent.NIP = contractor.NIP;
                            kontrahent.Nazwa = contractor.Nazwa;
                            kontrahent.EMAIL = contractor.Email;
                            kontrahent.Kod = contractor.Id;
                            kontrahent.PESEL = contractor.PESEL;

                            transaction.Commit();

                        }

                        session.Save();

                        KMKontrahent.Logger.Log("Pomyślnie zaimportowano dane kontrahenta o id " + contractor.Id, LogType.Information);
                        results.Add(result);

                    }


                }
                return results;

            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas importowania informacji o kontrahencie do systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return new List<Result>();
            }
        }

        public List<Result> ImportProducts(List<Product> products)
        {
            try
            {
                var results = new List<Result>();
                foreach (Product product in products)
                {

                    using (Session session = _login.CreateSession(false, false, "ImportProducts"))
                    {
                        var handelModule = HandelModule.GetInstance(session);
                        var crmModule = CRMModule.GetInstance(session);

                        var result = new Result
                        {
                            Record = product,
                            Success = true
                        };
                        Towar towar = null;
                        try
                        {
                            towar = handelModule.Towary.Towary.WgKodu[product.Kod];

                        }
                        catch (Exception exception)
                        {

                        }

                        using (ITransaction transaction = session.Logout(true))
                        {
                            if (towar == null)
                            {
                                towar = new Towar();

                                handelModule.Towary.Towary.AddRow(towar);

                            }
                            towar.Nazwa = product.Nazwa;
                            towar.EAN = product.EAN;
                            towar.Kod = product.Kod;
                            towar.DefinicjaStawki = CoreModule.GetInstance(session).DefStawekVat.WgKodu["23%"];
                            towar.DefinicjaStawkiZakupu = CoreModule.GetInstance(session).DefStawekVat.WgKodu["23%"];



                            transaction.Commit();

                        }

                        session.Save();

                        KMKontrahent.Logger.Log("Pomyślnie zaimportowano dane towaru o kodzie " + product.Kod, LogType.Information);
                        results.Add(result);

                    }


                }
                return results;

            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas importowania informacji o kontrahencie do systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return new List<Result>();
            }
        }
        #endregion
    }
}
