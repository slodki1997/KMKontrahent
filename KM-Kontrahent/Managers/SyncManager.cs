using KM_Kontrahent.Models;
using itDesk.Business.Debuggers.Enums;
using itDesk.Business.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

using Enova365Module = KM_Kontrahent.Modules.enova365.Module;
using Timer = System.Timers.Timer;

namespace KM_Kontrahent.Managers
{
    public class SyncManager
    {
        #region -- CONSTANTS --

        private const Int32 TIMER_INTERVAL = 60000;

        #endregion

        #region -- FIELDS --

        private Thread _syncThread;

        private readonly DirectoryManager _directoryManager;

        private readonly ManualResetEvent _workflow;

        private readonly Timer _syncTimer;

        #endregion

        #region -- PROPERTIES --

        public Enova365Module Enova365 { get; }


        public SettingsManager<Settings> SettingsManager { get; set; }

        #endregion

        #region -- CONSTRUCTORS --


        public SyncManager()
        {
            SettingsManager = new SettingsManager<Settings>(KMKontrahent.DirectoryPath);

            Enova365 = new Enova365Module();

            _directoryManager = new DirectoryManager(SettingsManager.Settings.Modules);
            _workflow = new ManualResetEvent(false);


            _syncTimer = new Timer(SettingsManager.Settings.SyncInterval * TIMER_INTERVAL);

            _syncTimer.Elapsed += SyncTimer_Elapsed;
        }

        #endregion

        #region -- METHODS --

        private Boolean ShouldApplicationBeStopped() => _workflow.WaitOne(5);

        private void InitializeSync()
        {
            Sync();

            _syncTimer.Start();
        }

        private void Logout() => Enova365?.Logout();

        public void Launch(Boolean isService)
        {
            _syncThread = new Thread(InitializeSync)
            {
                IsBackground = true,
                Name = "SyncThread"
            };

            _syncThread.SetApartmentState(ApartmentState.STA);
            _syncThread.Start();

            if (isService)
                return;

            while (!ShouldApplicationBeStopped()) { }
        }
        private void Login()
        {
            if (!Enova365.Login())
                throw new Exception("Nie udało się zalogować do systemu enova365.");

        }

        public void Stop()
        {
            Enova365?.Dispose();

            _workflow.Set();

            if (_syncThread.Join(5000))
                _syncThread.Abort();
        }

        private void Sync()
        {
            KMKontrahent.Logger.LogHeader("Rozpoczęto przesyłanie danych pomiędzy komputerem a systemem Enova365.");

            try
            {
                Login();

                if(Environment.UserInteractive)
                    ViewOptions();
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd,", LogType.Error);
                KMKontrahent.Logger.Log(exception);
            }


            KMKontrahent.Logger.LogHeader("Zakończono przesyłanie danych pomiędzy systemem komputerem lokalnym a systemem Enova365.");
        }

        private void ViewOptions()
        {
            string option;
            Console.WriteLine("Jaka symulacja Cię interesuje?");
            Console.WriteLine("-ke - Eksport kontrahnentów");
            Console.WriteLine("-te - Eksport towarów");
            Console.WriteLine("-ki - Import kontrahnentów");
            Console.WriteLine("-ti - Import towarów");
            while (true)
            {
                option = Console.ReadLine();
                if (option != "-ke" && option != "-te" && option != "-ki" && option != "-ti")
                    Console.WriteLine("Błędna opcja. Wybierz coś innego.");

                switch (option)
                {
                    case "-ke":
                        ExportKontrahentFromEnova365();
                        break;

                    case "-te":
                        ExportTowarFromEnova365();
                        break;

                    case "-ki":
                        ImportKontrahentToEnova365();
                        break;

                    case "-ti":
                        ImportTowarToEnova365();
                        break;

                }
            }

        }


        ////Enova -> XML

        private void ExportKontrahentFromEnova365()
        {
            try
            {
                KMKontrahent.Logger.Log("Rozpoczęto eksportowanie danych o kontrahentach z systemu enova365.", LogType.Information);
                List<Contractor> contractors = Enova365.GetContractor();
                _directoryManager.SaveContractorsToExportFromEnova365(contractors);

            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas eksportowania kontrahentów z systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);
            }
            finally
            {
                KMKontrahent.Logger.Log("Zakończono eksportowanie kontrahentów z systemu enova365.", LogType.Information);
            }


        }

        private void ExportTowarFromEnova365()
        {
            try
            {
                KMKontrahent.Logger.Log("Rozpoczęto eksportowanie danych o towarach z systemu enova365.", LogType.Information);
                List<Product> products = Enova365.GetProducts();
                _directoryManager.SaveProductsToExportFromEnova365(products);

            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas eksportowania towarów z systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);
            }
            finally
            {
                KMKontrahent.Logger.Log("Zakończono eksportowanie towarów z systemu enova365.", LogType.Information);
            }
        }


        // XML -> Enova365
        private void ImportKontrahentToEnova365()
        {

            try
            {
                KMKontrahent.Logger.LogHeader("Rozpoczęto importowanie danych o kontrahentach.");

                List<Contractor> contractors = _directoryManager.LoadContractorsToImportToEnova365();

                List<Result> results = Enova365.ImportContractors(contractors);

                //foreach (Result result in results.Where(result => result.Success))
                //    _directoryManager.ArchiveContractorsImportedToEnova365(result.Record);
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas importowania kontrahentów do systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);
            }
            finally
            {
                KMKontrahent.Logger.Log("Zakończono importowanie kontrahentów do systemu enova365.", LogType.Information);
            }

        }

        private void ImportTowarToEnova365()
        {
            try
            {
                KMKontrahent.Logger.LogHeader("Rozpoczęto importowanie danych o towarach.");

                List<Product> products = _directoryManager.LoadProductsToImportToEnova365();

                List<Result> results = Enova365.ImportProducts(products);

                //foreach (Result result in results.Where(result => result.Success))
                //    _directoryManager.ArchiveContractorsImportedToEnova365(result.Record);
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas importowania kontrahentów do systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);
            }
            finally
            {
                KMKontrahent.Logger.Log("Zakończono importowanie kontrahentów do systemu enova365.", LogType.Information);
            }

        }



        #endregion

        #region -- HANDLERS --

        private void SyncTimer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            try
            {
                _syncTimer.Stop();

                Sync();
            }
            finally
            {
                _syncTimer.Start();
            }
        }

        #endregion
    }
}
