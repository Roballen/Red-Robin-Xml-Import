using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using RedRobin.DataAccess.Models;
using RedRobin.DataAccess.Models.Generated;
using Utilities;

namespace RedRobin.DataAccess
{
    public class MatchedItemRepository_pp
    {
        public List<XmlMenuItemsXref> GetMatchedMenuItems()
        {
            var db = GetDataBase();

            return db.Fetch<XmlMenuItemsXref>(Sql.Builder.Append("Select XmlMenuItemsXref.* from XmlMenuItemsXref where menuitemguid IS NOT NULL and ")
                .Append("(select COUNT(*) from XmlIngredientsXref ")
                .Append(
                    "where XmlMenuItemsXref.xmlguid = XmlIngredientsXref.xmlmenuitemguid and XmlIngredientsXref.ingredientsguid IS NULL) = 0"));
        }

        public List<XmlMenuItemsXref> GetPartialMenuItems()
        {
            var db = GetDataBase();

            return db.Fetch<XmlMenuItemsXref>( Sql.Builder.Append("Select XmlMenuItemsXref.* from XmlMenuItemsXref where menuitemguid IS NOT NULL and ")
                .Append("(select COUNT(*) from XmlIngredientsXref ")
                .Append("where XmlMenuItemsXref.xmlguid = XmlIngredientsXref.xmlmenuitemguid and XmlIngredientsXref.ingredientsguid IS NULL) > 0"));

        }

        public List<XmlMenuItemsXref> GetUnMatchedMenuItems()
        {
            var db = GetDataBase();
            return db.Fetch<XmlMenuItemsXref>("Select * from XmlMenuItemsXref where menuitemguid IS NULL");
        }

        public XmlMenuItem GetXmlMenuItem(string xmlguid)
        {
            var db = GetDataBase();
            return db.Fetch<XmlMenuItem>(" where itemguid=@0", xmlguid).SingleOrDefault();
        }

        public XmlMenuItem GetXmlMenuItemWithIngredients(string xmlguid)
        {
            var menuitem = GetXmlMenuItem(xmlguid);

            if (menuitem != null)
                menuitem.Ingredients.AddRange(GetXmlImportIngredients(menuitem.XmlImportMenuItemId.ToString()));
            return menuitem;
        }

        public List<XmlIngredient> GetXmlImportIngredients(string xmlmenuitemid)
        {
            return GetDataBase().Fetch<XmlIngredient>(" where XmlImportMenuItemId=@0", xmlmenuitemid);
        }

        public MenuItem GetMenuItem(string guid)
        {
            var db = GetDataBase();
            return db.Fetch<MenuItem>(" where DataObjectId=@0", guid).SingleOrDefault();
        }

        /// <summary>
        /// get menu item with ingredients and nutrients
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public MenuItemAggregate GetMenuItemWithIngredients(string guid)
        {
            var db = GetDataBase();
            var item = db.Fetch<MenuItemAggregate>(" where DataObjectId=@0", guid).SingleOrDefault();

            if (item != null)
                item.Ingredients.AddRange(GetMatchedIngredients(item.menuitemid.ToString()));

            return item;

        }

        /// <summary>
        /// get all ingredients with nutrients for given menu id
        /// </summary>
        /// <param name="menuid"></param>
        /// <returns></returns>
        public List<IngredientAggregate> GetMatchedIngredients(string menuid)
        {
            var ingredients = GetDataBase().Fetch<IngredientAggregate>(Sql.Builder
                    .Append("SELECT Ingredients.* ")
                    .Append("FROM Ingredients")
                    .Append("INNER JOIN MenuItemIngredients ON MenuItemIngredients.ingredientid = Ingredients.ingredientid")
                    .Append("where MenuItemIngredients.menuitemid= " + menuid + " and Ingredients.active = 1")
                    );

            foreach (var ingredientAggregate in ingredients)
            {
                ingredientAggregate.Nutrients.AddRange(GetNutritionFacts(ingredientAggregate.ingredientid.ToString()));
            }

            return ingredients;
        }

        /// <summary>
        /// Gets all nutrition facts for given ingredient id
        /// </summary>
        /// <param name="ingredientid"></param>
        /// <returns></returns>
        public List<NutrientDisplay> GetNutritionFacts(string ingredientid)
        {
            return GetDataBase().Fetch<NutrientDisplay>(" where ingredientid=@0", ingredientid);
        }

        /// <summary>
        /// gets a dimple list of ingredients for dataobjectid
        /// </summary>
        /// <param name="DataObjectId"></param>
        /// <returns></returns>
        public IngredientAggregate GetMatchedIngredient(string DataObjectId, bool aggregate)
        {
            var result = GetDataBase().Fetch<IngredientAggregate>(" where DataObjectId=@0", DataObjectId).SingleOrDefault();
            if (aggregate && result != null)
                result.Nutrients.AddRange(GetNutritionFacts(result.ingredientid.ToString()));
            return result;
        }

        public XmlIngredientsXref GetIngredientCrossReference(string xmlguid)
        {
            return GetDataBase().Fetch<XmlIngredientsXref>(" where xmlguid=@0", xmlguid).SingleOrDefault();
        }

        public XmlMenuItemsXref GetMenuCrossReference(string xmlguid)
        {
            return GetDataBase().Fetch<XmlMenuItemsXref>(" where xmlguid=@0", xmlguid).SingleOrDefault();
        }

        public XmlMenuItemsXref GetMenuCrossReference(int itemid)
        {
            return GetDataBase().Fetch<XmlMenuItemsXref>(" where idpk=@0", itemid).SingleOrDefault();
        }

        public XmlMenuItem GetXmlImportMenuItemByMenuItemId(string xmlmenuitem)
        {
            return GetDataBase().Fetch<XmlMenuItem>(" where XmlImportMenuItemId=@0", xmlmenuitem).SingleOrDefault();
        }
        public List<IngredientCandidate> GetIngredientCandidatesByName(string name)
        {
            name = name.AlphabetOnly(true, false);
            var splitz = name.Split(' ');
            var orderedlist = splitz.OrderBy(s => s.Length);
            var builder = Sql.Builder.Append("Select *, NutritionFacts.value as Calories from Ingredients inner join NutritionFacts on NutritionFacts.ingredientid = Ingredients.ingredientid and NutritionFacts.nutritionfacttypeid = 3 where Ingredients.active=1 AND ");
            bool first = true;

            foreach (var s in orderedlist)
            {
                if (!s.IsEmpty() && !("RRGB,MIX").Contains(s.ToUpper()))
                {
                    if (!first)
                        builder = builder.Append(" OR ");

                    first = false;
                    builder = builder.Append("Ingredients.displayname LIKE '%[^a-z]" + s + "[^a-z]%' OR Ingredients.displayname LIKE '" + s + "[^a-z]%' OR Ingredients.displayname LIKE '%[^a-z]" + s + "'");
                }
            }

            return GetDataBase().Fetch<IngredientCandidate>(builder);

        }

        public List<IngredientCandidate> GetIngredientCandidatesByPrevious(string menuguid)
        {
            return GetDataBase().Fetch<IngredientCandidate>(
                Sql.Builder.Append("Select Ingredients.*,NutritionFacts.value as Calories from Ingredients ")
                .Append("inner join MenuItemIngredients on MenuItemIngredients.ingredientid = Ingredients.ingredientid ")
                .Append("inner join NutritionFacts on NutritionFacts.ingredientid = Ingredients.ingredientid and NutritionFacts.nutritionfacttypeid = 3")
                .Append("where Ingredients.active = 1 and MenuItemIngredients.menuitemid = (select menuitemid from menuitems where dataobjectid = '" + menuguid + "')")
                );
        }

        public IngredientCandidates GetIngredientCandidates(XmlImportIngredient ingredient)
        {
            var candidates = new IngredientCandidates();

            candidates.ByNameCandidates = GetIngredientCandidatesByName(ingredient.ItemIngredientName);

            var xmlmenuitem = GetXmlImportMenuItemByMenuItemId(ingredient.XmlImportMenuItemId.ToString());
            if (xmlmenuitem != null)
            {
                var menuxref = GetMenuCrossReference(xmlmenuitem.ItemGuid);
                if (menuxref != null)
                    candidates.ByPreviousCandidates = GetIngredientCandidatesByPrevious(menuxref.menuitemguid.ToString());
            }
            return candidates;
        }

        public XmlIngredient GetXmlIngredient(string xmlingredientid)
        {
            return GetDataBase().Fetch<XmlIngredient>(" where xmlimportIngredientId=@0", xmlingredientid).SingleOrDefault();
        }

        public XmlIngredient GetXmlIngredientByGuid(string xmlguid)
        {
            return GetDataBase().Fetch<XmlIngredient>(" where ItemIngredientGuid=@0", xmlguid).SingleOrDefault();
        }

        private Database GetDataBase()
        {
            return new Database("RedRobin");
        }

        public bool UpdateIngredientXref(string xmlguid, string guid)
        {
            try
            {
                var xref = GetIngredientCrossReference(xmlguid);
                if (xref == null)
                    return false;

                var ingredient = GetMatchedIngredient(guid, false);
                if (ingredient == null)
                    return false;

                xref.ingredientsguid = new Guid(guid);
                xref.igredientname = ingredient.managename;

                if (xref.igredientname.IsEmpty())
                    xref.igredientname = ingredient.displayname;

                xref.Save();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public MenuItemCandidates GetMenuItemCandidates(XmlMenuItem xmlmenuitem)
        {
            var candidates = new MenuItemCandidates();

            candidates.ByNameCandidates = GetMenuItemCandidatesByName(xmlmenuitem.ItemName);
            return candidates;
        }

        public List<MenuItem> GetMenuItemCandidatesByName(string name)
        {
            name = name.AlphabetOnly(true, false);
            var splitz = name.Split(' ');
            var orderedlist = splitz.OrderBy(s => s.Length);
            var builder = Sql.Builder.Append("Select * from MenuItems where ");
            bool first = true;

            foreach (var s in orderedlist)
            {
                if (!s.IsEmpty() && !("RRGB,MIX").Contains(s.ToUpper()))
                {
                    if (!first)
                        builder = builder.Append(" OR ");

                    first = false;
                    builder = builder.Append("name LIKE '%[^a-z]" + s + "[^a-z]%' OR name LIKE '" + s + "[^a-z]%' OR name LIKE '%[^a-z]" + s + "'");
                }
            }

            return GetDataBase().Fetch<MenuItem>(builder);
        }

        public bool UpdateMenuItemsXref(string xmlguid, string matchedGuid)
        {
            try
            {
                var xref = GetMenuCrossReference(xmlguid);
                if (xref == null)
                    return false;

                var menuitem = GetMenuItem(matchedGuid);
                if (menuitem == null)
                    return false;

                xref.menuitemguid = new Guid(matchedGuid);
                xref.menuitemname = menuitem.name;

                xref.Save();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public XmlImport GetImport()
        {
            return GetDataBase().Fetch<XmlImport>("select * from xmlimports").SingleOrDefault();
        }


        private void UpdateIngredientNutritionalInformation(UpdateObject update, ref UpdateResults results)
        {
            // need to get the xmlimportitem

            var xmlimport = GetXmlIngredientByGuid(update.xmlingredientguid.ToString());

            if ( xmlimport == null)
                throw new ApplicationException("XmlImportIngredient not found");

            //need to blow out the previous facts
            GetDataBase().Execute(" update NutritionFacts set active = 0 where ingredientid =@0", update.ingredientid);

            //translate field to nutrition type
            foreach (var facttype in GetDataBase().Fetch<NutritionFactType>(" select * from NutritionFactTypes"))
            {
                try
                {
                    var fact = GetNutritionFact(update.ingredientid, facttype.nutritionfacttypeid);
                    if (fact == null)
                    {
                        fact = new NutritionFact();
                        fact.ingredientid = update.ingredientid;
                        fact.nutritionfacttypeid = facttype.nutritionfacttypeid;
                        fact.ServiceObjectId = new Guid();
                        fact.DataObjectId = new Guid();
                    }

                    fact.active = true;
                    fact.value = GetFactValueFromImport(xmlimport, facttype.nutritionfacttypeid);

                    if (fact.IsNew())
                        fact.Insert();
                    else
                        fact.Update();
                    results.NutrionalFactsUpdates++;
                }
                catch (Exception e)
                {
                    results.Errors.Add("NutrionalFact:" + facttype.name + " for Ingredient:" + update.ingredientid, e.Message);
                }
            }

        }

        private string GetFactValueFromImport( XmlImportIngredient xmlimport, int factid )
        {
            switch (factid)
            {
                case 1:
                    return xmlimport.Calories;
                case 3:
                    return xmlimport.Quantity;
                case 4:
                    return xmlimport.Fat;
                case 5:
                    return xmlimport.SaturatedFat;
                case 6:
                    return xmlimport.Sodium;
                case 7:
                    return xmlimport.Carbohydrates;
                case 8:
                    return xmlimport.Protein;
                case 9:
                    return xmlimport.Sugar;
                case 10:
                    return xmlimport.Cholesterol;
                case 11:
                    return xmlimport.FatCalories;
                case 12:
                    return xmlimport.Fiber;
                case 13:
                    return xmlimport.TransFat;
                default:
                    return "";
            }
        }

        public NutritionFact GetNutritionFact(int ingredientid, int factid)
        {
            return
                GetDataBase().Fetch<NutritionFact>(
                    " select * from NutritionFacts where ingredientid=@0 and nutritionfacttypeid=@1", ingredientid,
                    factid).SingleOrDefault();
        }

        public UpdateResults UpdateMenu()
        {
            var UpdateResults = new UpdateResults();

            var thelist = GetMatchedMenuItemIds();

            //scope by menu id
            foreach (var menuid in thelist)
            {
                try
                {
                    var updates = GetDynamicXrefQuery(menuid);
                    GetDataBase().Execute("Delete from MenuItemIngredients where menuitemid = " + menuid);
                    foreach (var update in updates)
                    {
                        UpdateIngredientXref(update);
                        try
                        {
                            UpdateIngredientNutritionalInformation(update, ref UpdateResults);
                        }
                        catch (Exception e)
                        {
                            UpdateResults.Errors.Add("Ingredient Nutrition Update: " + update.ingredientid, e.Message);
                        }
                        UpdateResults.IngredientsUpdated++;
                    }
                    UpdateResults.MenuItemsUpdated++;
                }
                catch (Exception e)
                {
                    UpdateResults.Errors.Add("Menu Item Update: " + menuid,e.Message);
                }
            }
          
            return UpdateResults;

        }

        private List<int> GetMatchedMenuItemIds()
        {
            var thelist = new List<int>();
            var builder = Sql.Builder.Append(" Select ")
             .Append(" Distinct(MenuItems.menuitemid)")
             .Append(" from XmlMenuItemsXref ")
             .Append(" left outer join MenuItems on XmlMenuItemsXref.menuitemguid = MenuItems.DataObjectId")
             .Append(" left outer join XmlIngredientsXref on  XmlMenuItemsXref.xmlguid = XmlIngredientsXref.xmlmenuitemguid")
             .Append(" where menuitemguid IS NOT NULL and ")
             .Append(" (select COUNT(*) from XmlIngredientsXref where XmlMenuItemsXref.xmlguid = XmlIngredientsXref.xmlmenuitemguid and XmlIngredientsXref.ingredientsguid IS NULL) = 0")
             .Append(" order by menuitemid");

            foreach (var arg in GetDataBase().Fetch<dynamic>(builder))
            {
                thelist.Add(arg.menuitemid);
            }

            return thelist;

        }

        /// <summary>
        /// need to decide how to handle the
        /// </summary>
        /// <param name="upd"></param>
        private void UpdateIngredientXref(UpdateObject upd)
        {
                var linkedingredients = new MenuItemIngredient();
                linkedingredients.menuitemingredientid = upd.xrefpk;
                linkedingredients.menuitemid = upd.menuid;
                linkedingredients.ingredientid = upd.ingredientid;
                linkedingredients.sortindex = upd.sortindex;
                linkedingredients.ServiceObjectId = upd.ServiceObjectId ?? new Guid();
                linkedingredients.DataObjectId = upd.DataObjectId ?? new Guid();
                linkedingredients.Insert();
        }

        public List<UpdateObject> GetDynamicXrefQuery( int menuid )
        {
            var builder = Sql.Builder.Append(" Select XmlMenuItemsXref.xmlguid as xmlmenuguid,")
                .Append(" XmlMenuItemsXref.menuitemguid as menuguid,")
                .Append(" XmlIngredientsXref.[xmlguid] as xmlingredientguid,")
                .Append(" XmlIngredientsXref.ingredientsguid as ingredientguid,")
                .Append(" Ingredients.ingredientid as ingredientid,")
                .Append(" MenuItems.menuitemid as menuid,")
                .Append(" MenuItemIngredients.sortindex,")
                .Append(" MenuItemIngredients.menuitemingredientid as xrefpk,")
                .Append(" MenuItemIngredients.DataObjectId,")
                .Append(" MenuItemIngredients.ServiceObjectId")
                .Append(" from XmlMenuItemsXref ")
                .Append(" left outer join MenuItems on XmlMenuItemsXref.menuitemguid = MenuItems.DataObjectId")
                .Append(
                    " left outer join XmlIngredientsXref on  XmlMenuItemsXref.xmlguid = XmlIngredientsXref.xmlmenuitemguid")
                .Append(" left outer join Ingredients on Ingredients.DataObjectId = XmlIngredientsXref.ingredientsguid")
                .Append(
                    " left outer join MenuItemIngredients on ( Ingredients.ingredientid = MenuItemIngredients.ingredientid and MenuItems.menuitemid = MenuItemIngredients.menuitemid )")
                .Append(" where MenuItems.menuitemid = " + menuid);

            var dynamo = GetDataBase().Fetch<UpdateObject>(builder);
            return dynamo;


        }

        public DuplicateManager GetXmlIngredientDuplicates()
        {
            DuplicateManager manager = new DuplicateManager();
            var dups = GetDataBase().Fetch<XmlImportIngredient>(
                    " SELECT ItemIngredientName, Calories, Quantity, COUNT(ItemIngredientName) AS NumOccurrences FROM XmlImportIngredients GROUP BY ItemIngredientName, Calories, Quantity HAVING ( COUNT(ItemIngredientName) > 1 ) order by itemingredientname");
            foreach (var xmlImportIngredient in dups)
            {
                var items = GetXmlIngredientsByName(xmlImportIngredient.ItemIngredientName, xmlImportIngredient.Calories);
                List<IngredientAggregate> ingredients = new List<IngredientAggregate>();
                foreach (var importIngredient in items)
                {
                    var xref = GetIngredientCrossReference(importIngredient.ItemIngredientGuid);
                    if ( xref != null && xref.ingredientsguid != null)
                    {
                        var ingdredient = GetMatchedIngredient(xref.ingredientsguid.ToString(), true);
                        if ( ingdredient != null)
                            ingredients.Add(ingdredient);
                    }

                }
                var dup = new Duplicate();
                dup.Name = xmlImportIngredient.ItemIngredientName;
                dup.DisplayName = xmlImportIngredient.ItemIngredientName + " with " + xmlImportIngredient.Calories + " calories";
                dup.NumberOfDups = items.Count;
                manager.Duplicates.Add(dup, ingredients);
            }
            return manager;

        }

        public DuplicateManager GetDuplicates()
        {
            DuplicateManager manager = new DuplicateManager();
            var dups = GetDataBase().Fetch<DuplicateIngredients>( " SELECT ItemIngredientName, Calories, Quantity, COUNT(ItemIngredientName) AS NumOccurrences FROM XmlImportIngredients GROUP BY ItemIngredientName, Calories, Quantity HAVING ( COUNT(ItemIngredientName) > 1 ) order by itemingredientname");
            foreach (var xmlImportIngredient in dups)
            {
                //this get all of the ingredient ids where there is an xref and there is a name and calories match
                List<IngredientAggregate> ingredients = new List<IngredientAggregate>();
                var candidates = GetDuplicateIngredientIds(xmlImportIngredient.Calories,  xmlImportIngredient.ItemIngredientName, xmlImportIngredient.Quantity);
                
                if ( candidates.Count < xmlImportIngredient.NumOccurrences ) //means there is a null and needs update
                {
                    List<string> added = new List<string>();
                    foreach (var ingredientcandidate in candidates)
                    {
                        if ( ! added.Contains(ingredientcandidate.DataObjectId.ToString()))
                        {
                            var ingdredient = GetMatchedIngredient(ingredientcandidate.DataObjectId.ToString(), true);
                            if (ingdredient != null)
                            {
                                added.Add(ingredientcandidate.DataObjectId.ToString());
                                ingredients.Add(ingdredient);
                            }
                        }
                    }
                }

                if ( ingredients.Count > 0 || candidates.Count == 0)
                {
                    var dup = new Duplicate();
                    dup.Name = xmlImportIngredient.ItemIngredientName;
                    dup.Calories = xmlImportIngredient.Calories;
                    dup.Quantity = xmlImportIngredient.Quantity;
                    dup.DisplayName = xmlImportIngredient.ItemIngredientName + " with " + xmlImportIngredient.Calories + " calories";
                    dup.NumberOfDups = xmlImportIngredient.NumOccurrences;
                    manager.Duplicates.Add(dup, ingredients);
                }
            }
            return manager;

        }

        public List<Ingredient> GetDuplicateIngredientIds(string calories, string ItemName, string quantity)
        {
            ItemName = ItemName.Replace("'", "''");
            return
                GetDataBase().Fetch<Ingredient>(Sql.Builder.Append(
                    "select Ingredients.DataObjectId from XmlImportIngredients ").Append
                                                    ("left outer join XmlIngredientsXref on ").Append
                                                    ("XmlIngredientsXref.xmlguid = XmlImportIngredients.ItemIngredientGuid ")
                                                    .Append
                                                    ("left outer join Ingredients ").Append
                                                    ("on XmlIngredientsXref.ingredientsguid = Ingredients.DataObjectId ")
                                                    .Append
                                                    ("where XmlImportIngredients.ItemIngredientName ='"+ ItemName +"' ")
                                                    .Append
                                                    ("and XmlImportIngredients.Quantity='" + quantity + "' and XmlImportIngredients.Calories='" + calories + "' and XmlIngredientsXref.ingredientsguid IS NOT NULL "));
        }

        public List<XmlImportIngredient> GetXmlIngredientsByName(string name, string calories)
        {
            return
                GetDataBase().Fetch<XmlImportIngredient>(
                    " select * from XmlImportIngredients where ItemIngredientName =@0 and Calories=@1", name,calories);
        }



        public bool UpdateXrefIngredients(string itemname, string quantity, string calories, string itemguid)
        {
            try
            {
                var updates = GetDataBase().Fetch<XmlImportIngredient>( " select * from XmlImportIngredients where ItemIngredientName =@0 and Calories=@1 and Quantity=@2", itemname, calories, quantity);
                foreach (var xmlImportIngredient in updates)
                {
                    var ingredient = GetMatchedIngredient(itemguid, false);

                    if (GetDataBase().Execute(" Update XmlIngredientsXref set ingredientsguid='" + itemguid +
                                          "', igredientname='" + ingredient.managename + "' where xmlguid='" + xmlImportIngredient.ItemIngredientGuid + "'") != 1)
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
