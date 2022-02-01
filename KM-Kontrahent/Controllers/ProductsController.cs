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
    public class ProductsController : ApiController
    {

        [HttpGet]
        public IHttpActionResult Get()
        {
            List<Product> productsList;
            productsList = KMKontrahent.SyncManager.Enova365.GetProducts();

            return Json(productsList);
        }

        [HttpPost]
        public string EditPost([FromBody] List<Product> products)
        {
            List<Result> results;
            results = KMKontrahent.SyncManager.Enova365.ImportProducts(products);
            return "Dodano pomyślnie";
        }
    }
}
