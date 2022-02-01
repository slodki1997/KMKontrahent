using KM_Kontrahent.Models.Abstracts;
using System;

namespace KM_Kontrahent.Models
{
    public class Result
    {
        #region -- PROPERTIES --

        public Boolean Success { get; set; }

        public IRecord Record { get; set; }

        #endregion
    }
}
