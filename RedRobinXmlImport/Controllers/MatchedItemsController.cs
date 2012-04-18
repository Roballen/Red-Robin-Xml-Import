using System;
using System.Web.Mvc;
using RedRobin.DataAccess;
using RedRobin.DataAccess.Models;

namespace RedRobinXmlImport.Controllers
{
    public class MatchedItemsController : Controller
    {
        //
        // GET: /MatchedItems/

        public ActionResult MatchedMenuItems()
        {
            var repo = new MatchedItemRepository_pp();
            var model = repo.GetMatchedMenuItems();
            ViewBag.Total = model.Count;
            return View("MatchedMenuItems", model);
        }

        public ActionResult PartialMatchMenuItems()
        {
            var repo = new MatchedItemRepository_pp();
            var model = repo.GetPartialMenuItems();
             ViewBag.Total = model.Count;
            return View("MatchedMenuItems", model);
        }

        public ActionResult UnmatchedMenuItems()
        {
            var repo = new MatchedItemRepository_pp();
            var model = repo.GetUnMatchedMenuItems();
            ViewBag.Total = model.Count;
            return View("MatchedMenuItems", model);
        }

        public ActionResult MatchAggregate(string xmlguid, string menuitemguid)
        {
            var repo = new MatchedItemRepository_pp();
            var aggregate = new XmlMenuItem();

            if ( !string.IsNullOrEmpty(xmlguid))
            {
                aggregate = repo.GetXmlMenuItemWithIngredients(xmlguid);

                foreach (var xmlIngredient in aggregate.Ingredients)
                {
                    //need to get ingredient match, different from menu item ingredients
                    var xref = repo.GetIngredientCrossReference(xmlIngredient.ItemIngredientGuid);
                    if (xref != null && xref.ingredientsguid != null) // means there is no match
                        xmlIngredient.IngredientMatch = repo.GetMatchedIngredient(xref.ingredientsguid.ToString(),true);
//                  
//                    if ( xmlIngredient.IngredientMatch == null)
//                        xmlIngredient.IngredientMatch = new IngredientAggregate();

                }
            }

            if (string.IsNullOrEmpty(menuitemguid))
            {
                //attempt to get from xref
                var xref = repo.GetMenuCrossReference(xmlguid);
                menuitemguid = xref.menuitemguid.ToString();
            }
            if (!string.IsNullOrEmpty(menuitemguid))
                aggregate.MenuItemMatch = repo.GetMenuItemWithIngredients(menuitemguid);


            return View(aggregate);
        }

        public ActionResult GetIngredientDetails(string ingredientId)
        {
            var repo = new MatchedItemRepository_pp();
            var results = repo.GetMatchedIngredient(ingredientId, false);
            return View(results);
        }

        public ActionResult IngredientMatchControl(string xmlingredientid)
        {
            try
            {
                IngredientCandidates candidates = new IngredientCandidates();
                var repo = new MatchedItemRepository_pp();
                var xmlingredient = repo.GetXmlIngredient(xmlingredientid);
                if ( xmlingredient != null )
                    candidates = repo.GetIngredientCandidates(xmlingredient);
                candidates.XmlIngredient = xmlingredient;
                return View (candidates);
            }
            catch (Exception e)
            {
                return Content("<p><div class=\"error-message\">Unable to find candidates, verify the menu item is matched.</div></p>");
            }
        }

        public ActionResult MenuItemMatchControl(string xmlmenuitemid)
        {
            MenuItemCandidates candidates = new MenuItemCandidates();
            var repo = new MatchedItemRepository_pp();
            var xmlmenuitem = repo.GetXmlMenuItem(xmlmenuitemid);
            if (xmlmenuitem != null)
                candidates = repo.GetMenuItemCandidates(xmlmenuitem);
            candidates.XmlMenuItem = xmlmenuitem;
            return View(candidates);
        }

        public ActionResult UpdateIngredientMatch(string xmlguid,string matchedGuid)
        {
            var repo = new MatchedItemRepository_pp();
            var response = "Ingredient Updated to " + matchedGuid;
            try
            {
                if (!repo.UpdateIngredientXref(xmlguid, matchedGuid))
                    response = "Unable to update Ingredient";
            }
            catch (Exception e)
            {
                response = "Oops, an error occured. Details: " + e.Message;
            }

            var ingredient = repo.GetXmlIngredientByGuid(xmlguid);
            ingredient.IngredientMatch = repo.GetMatchedIngredient(matchedGuid, true);

            return PartialView("GetIngredientDetails", ingredient);

            return Content(response);
        }

        public ActionResult UpdateMenuItemsMatch(string xmlguid,string matchedGuid)
        {
            var repo = new MatchedItemRepository_pp();
            repo.UpdateMenuItemsXref(xmlguid, matchedGuid);

            var menuitem = repo.GetXmlMenuItem(xmlguid);
            menuitem.MenuItemMatch = repo.GetMenuItemWithIngredients(matchedGuid);
            return PartialView("GetMenuItemDetails", menuitem);
        }

        public ActionResult UpdateMenuItemMatch(string xmlmenuitem, string matchedguid)
        {
            XmlMenuItem menuitem = new XmlMenuItem();
            return PartialView("GetMenuItemDetails", menuitem);
        }
    }
}
