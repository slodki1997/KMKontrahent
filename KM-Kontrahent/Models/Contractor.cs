using KM_Kontrahent.Managers;
using KM_Kontrahent.Models.Abstracts;
using itDesk.Business.Debuggers.Enums;
using Newtonsoft.Json.Linq;
using Soneta.Business.Db;
using Soneta.Handel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace KM_Kontrahent.Models
{
    public class Contractor : IRecord
    {
        public String Id { get; set; }
        public String ModuleId { get; set; }
        public String Nazwa { get; set; }
        public String NIP { get; set; }
        public String Adres { get; set; }
        public String Email { get; set; }
        public String PESEL { get; set; }
        public String KodKraju { get; set; }
        public Boolean Rodzic { get; set; }



    }
}
