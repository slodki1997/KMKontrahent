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
    public class Product : IRecord
    {
        public String Id { get; set; }
        public String Guid { get; set; }
        public String ModuleId { get; set; }
        public String Kod { get; set; }
        public String Nazwa { get; set; }
        public String EAN { get; set; }
        public String DefinicjaStawki { get; set; }
    }
}
