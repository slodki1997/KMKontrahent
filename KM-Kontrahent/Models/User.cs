using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KM_Kontrahent.Models
{

    [TypeConverter(typeof(ExpandableObjectConverter))]

    public class User
    {
        #region -- PROPERTIES --

        [DefaultValue("")]
        [DisplayName("Login")]
        public String Name { get; set; } = String.Empty;

        [DefaultValue("")]
        [DisplayName("Hasło")]
        [PasswordPropertyText(true)]
        public String Password { get; set; } = String.Empty;

        #endregion
    }
}
