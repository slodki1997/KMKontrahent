using itDesk.Business.Debuggers.Enums;
using Soneta.Start;
using System;

namespace KM_Kontrahent.Modules.enova365
{
    public static class ModuleLoader
    {
        #region -- METHODS --

        public static Boolean Load()
        {
            try
            {
                var loader = new Loader
                {
                    WithExtensions = true,
                    WithUI = false

                };
                loader.Load();


                return true;
            }
            catch (Exception exception)
            {
                KMKontrahent.Logger.Log("Wystąpił błąd podczas ładowania bibliotek systemu enova365.", LogType.Error);
                KMKontrahent.Logger.Log(exception);

                return false;
            }
        }

        #endregion
    }
}
