using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using KM_Kontrahent.Models;
using Newtonsoft.Json;

namespace KM_Kontrahent.Controllers
{
    public class ContractorsController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get()
        {
            List<Contractor> contractorsList;
            contractorsList = KMKontrahent.SyncManager.Enova365.GetContractor();

            return Json(contractorsList);
        }

        [HttpPost]
        public string EditPost([FromBody] List<Contractor> contractors)
        {
            List<Result> results;
            results = KMKontrahent.SyncManager.Enova365.ImportContractors(contractors);
            return "Dodano pomyślnie";
        }
    }
}
