using System;

namespace KM_Kontrahent.Models.Abstracts
{
    public interface IRecord
    {
        #region -- PROPERTIES --

        String Id { get; set; }
        String ModuleId { get; set; }

        #endregion
    }
}
