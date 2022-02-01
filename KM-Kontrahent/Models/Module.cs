using System;
using System.ComponentModel;

namespace KM_Kontrahent.Models
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Module
    {
        #region -- PROPERTIES --

        [DefaultValue("")]
        [DisplayName("Identyfikator modułu")]
        public String Id { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Adres URL zasobu")]
        public String ResourceUrl { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Token")]
        public String Token { get; set; } = String.Empty;

        #endregion
    }
}
