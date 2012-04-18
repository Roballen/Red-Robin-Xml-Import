using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedRobin.DataAccess;

namespace RedRobinXmlImport.Controllers
{
    public class UpdateNutrionalInformationController : Controller
    {
        //
        // GET: /UpdateNutrionalInformation/
        public ActionResult Index()
        {
            var repo = new MatchedItemRepository_pp();
            
            return View(repo.GetImport());
        }

        public ActionResult UpdateNutritionalInformation()
        {
            var repo = new MatchedItemRepository_pp();
            var results = repo.UpdateMenu();
        
            return View(results);
        }

    }
}
