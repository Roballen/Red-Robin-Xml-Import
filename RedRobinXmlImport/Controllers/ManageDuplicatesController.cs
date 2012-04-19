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
            //var dups = repo.GetXmlIngredientDuplicates();
            var dups = repo.GetDuplicates();
            ViewBag.Total = dups.Duplicates.Count;
            return View(dups);
        }

        //will replace search and replace all matches of itemname in ingredientsxref with itemguid
        public ActionResult ShrinkDuplicates( string itemname, string itemguid,string calories, string quantity)
        {

            var repo = new MatchedItemRepository_pp();
            if (repo.UpdateXrefIngredients(itemname, quantity, calories, itemguid))
                return Content("<div class=\"notify\">All " + itemname + " updated to use " + itemguid + " as matched ingredient</div>");
            else
            {
                return Content("<div class=\"error-message\">Unable to update " + itemname + " to use " + itemguid + ", please refresh and try again.</div>");

            }
        }

    }
}
