using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedRobin.DataAccess;

namespace RedRobinXmlImport.Controllers
{
    public class TestUpdatesController : Controller
    {
        //
        // GET: /TestUpdates/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetExample(string guid)
        {
            var repo = new MatchedItemRepository_pp();
            var item =repo.GetMenuItem(guid);

            return View("Index",item);

        }

    }
}
