using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KM_Kontrahent.Models
{
    public class Settings
    {
        #region -- PROPERTIES --

        [DefaultValue(20)]
        [DisplayName("Odstęp synchronizacji")]
        public UInt32 SyncInterval { get; set; } = 10;

        [DisplayName("LiveKid - moduły")]
        public List<Module> Modules { get; set; } = new List<Module>();

        [DefaultValue("")]
        [DisplayName("Nazwa bazy")]
        public String Database { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Definicja dokumentu handlowego - symbol")]
        public String DokumentHandlowyDefinicjaSymbol { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Ewidencja - symbol")]
        public String EwidencjaSymbol { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Podstawa zwolnienia")]
        public String PodstawaZwolnieniaStawkaVat { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Sposób zapłaty - nazwa")]
        public String SposobZaplatyNazwa { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Obecność - nazwa domyślnej kartoteki")]
        public String TowarObecnoscDomyslnyNazwa { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Wyżywienie - nazwa domyślnej kartoteki")]
        public String TowarWyzywienieDomyslnyNazwa { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Stawka VAT - kod")]
        public String TowarStawkaVatKod { get; set; } = String.Empty;

        [DisplayName("Operator")]
        public User User { get; set; } = new User();

        #endregion
    }
}
