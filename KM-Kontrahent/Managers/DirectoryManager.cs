using KM_Kontrahent.Models;
using KM_Kontrahent.Models.Abstracts;
using itDesk.Business.Debuggers.Enums;
using itDesk.Xml.Comparison;
using itDesk.Xml.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.XmlDiffPatch;

namespace KM_Kontrahent.Managers
{
    public class DirectoryManager
    {
        #region -- CONSTANTS --

        private const String ARCHIVE = "Archive";
        private const String CONTRACTORS_DIRECTORY = "Contractors";
        private const String ENOVA365_MODULE = "enova365";
        private const String PRODUCTS_DIRECTORY = "Products";
        private const String ROOT_DIRECTORY = "AppData";

        #endregion

        #region -- PROPERTIES --

        public String DirectoryPath { get; }

        #endregion

        #region -- CONSTRUCTORS --

        public DirectoryManager(IEnumerable<Module> modules)
        {
            DirectoryPath = Path.Combine(KMKontrahent.DirectoryPath, ROOT_DIRECTORY);

            Initialize(modules);
        }

        #endregion

        #region -- METHODS --

        private void Initialize(IEnumerable<Module> modules)
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

        }

        private static Boolean Save(IRecord record, String path)
        {
            var serializer = new XmlSerializer<IRecord>();

            String xml = serializer.Serialize(record);

            String fileName = String.Format("{0}.xml", record.Id);
            String filePath = Path.Combine(path, fileName);

            if (WasIdenticalXmlFileSent(xml, path, fileName))
                return false;

            using (var writer = new StreamWriter(filePath))
                writer.WriteLine(xml);

            return true;
        }

        public void SaveContractorsToExportFromEnova365(List<Contractor> contractors)
        {
            String path = Path.Combine(DirectoryPath, CONTRACTORS_DIRECTORY);

            foreach (Contractor contractor in contractors)
            {
                try
                {
                    if (!Save(contractor, path))
                    {
                        KMKontrahent.Logger.Log($"Kontrahent '{contractor.Id}' zostanie pominięty, ponieważ nie został zmieniony od ostatniej synchronizacji.", LogType.Information);

                        continue;
                    }

                    KMKontrahent.Logger.Log($"Kontrahent '{contractor.Id}' został zapisany.", LogType.Information);
                }
                catch (Exception exception)
                {
                    KMKontrahent.Logger.Log($"Wystąpił błąd podczas zapisywania kontrahenta '{contractor.Id}'", LogType.Error);
                    KMKontrahent.Logger.Log(exception);
                }
            }
        }

        public void SaveProductsToExportFromEnova365(List<Product> products)
        {
            String path = Path.Combine(DirectoryPath, PRODUCTS_DIRECTORY);
            foreach (Product product in products)
            {
                try
                {
                    if (!Save(product, path))
                    {
                        KMKontrahent.Logger.Log($"Towar '{product.Kod}' zostanie pominięty, ponieważ nie został zmieniony od ostatniej synchronizacji.", LogType.Information);

                        continue;
                    }

                    KMKontrahent.Logger.Log($"Towar '{product.Kod}' został zapisany.", LogType.Information);
                }
                catch (Exception exception)
                {
                    KMKontrahent.Logger.Log($"Wystąpił błąd podczas zapisywania towaru '{product.Id}'", LogType.Error);
                    KMKontrahent.Logger.Log(exception);
                }
            }
        }

        public List<Contractor> LoadContractorsToImportToEnova365()
        {
            var contractors = new List<Contractor>();

            String path = Path.Combine(DirectoryPath, CONTRACTORS_DIRECTORY);

            var directory = new DirectoryInfo(path);


            FileInfo[] files = directory.GetFiles("*.xml");

            foreach (FileInfo file in files)
            {
                try
                {
                    var deserializer = new XmlDeserializer<Contractor>();

                    Contractor contractor = deserializer.Deserialize(file);

                    if (contractor == null)
                        throw new NullReferenceException("Nie udało się zdeserializować kontrahenta.");

                    contractors.Add(contractor);
                }
                catch (Exception exception)
                {
                    KMKontrahent.Logger.Log($"Wystąpił błąd podczas przetwarzania pliku '{file.Name}'.", LogType.Error);
                    KMKontrahent.Logger.Log(exception);
                }
            }
            return contractors;
        }

        public List<Product> LoadProductsToImportToEnova365()
        {
            var products = new List<Product>();

            String path = Path.Combine(DirectoryPath, PRODUCTS_DIRECTORY);

            var directory = new DirectoryInfo(path);


            FileInfo[] files = directory.GetFiles("*.xml");

            foreach (FileInfo file in files)
            {
                try
                {
                    var deserializer = new XmlDeserializer<Product>();

                    Product product = deserializer.Deserialize(file);

                    if (product == null)
                        throw new NullReferenceException("Nie udało się zdeserializować towaru.");

                    products.Add(product);
                }
                catch (Exception exception)
                {
                    KMKontrahent.Logger.Log($"Wystąpił błąd podczas przetwarzania pliku '{file.Name}'.", LogType.Error);
                    KMKontrahent.Logger.Log(exception);
                }
            }
            return products;
        }

        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
        private static Boolean WasIdenticalXmlFileSent(String xml, String directoryPath, String fileName)
        {
            try
            {
                String archivePath = Path.Combine(directoryPath, "Archive");

                var archiveDirectory = new DirectoryInfo(archivePath);

                String archiveFileName = String.Format("{0}.arch", fileName);

                FileInfo[] files = archiveDirectory.GetFiles(archiveFileName);

                FileInfo file = files.FirstOrDefault();

                if (file == null)
                    return false;

                var comparer = new XmlComparer();

                return comparer.Compare(file.FullName, xml, XmlDiffOptions.IgnoreChildOrder | XmlDiffOptions.IgnoreNamespaces | XmlDiffOptions.IgnorePrefixes);
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas porównywania dwóch struktur XML.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return false;
            }
        }

        private static void Archive(IRecord record, String path)
        {
            String fileName = String.Format("{0}.xml", record.Id);
            String filePath = Path.Combine(path, fileName);

            String archiveDirectoryPath = Path.Combine(path, ARCHIVE);

            String archiveFileName = String.Format("{0}.arch", fileName);
            String archiveFilePath = Path.Combine(archiveDirectoryPath, archiveFileName);

            if (File.Exists(archiveFilePath))
                File.Delete(archiveFilePath);

            File.Move(filePath, archiveFilePath);
        }


    }

    #endregion
}
