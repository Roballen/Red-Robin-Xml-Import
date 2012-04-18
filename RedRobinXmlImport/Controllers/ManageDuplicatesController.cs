using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedRobin.DataAccess;

namespace RedRobinXmlImport.Controllers
{
    public class ManageDuplicatesController : Controller
    {
        //
        // GET: /ManageDuplicates/

        public ActionResult Index()
        {
            var repo = new MatchedItemRepository_pp();
            var dups = repo.GetXmlIngredientDuplicates();
            ViewBag.Total = dups.Duplicates.Count;
            return View(dups);
        }

    }
}
