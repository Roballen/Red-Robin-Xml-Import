using System;
using System.Linq;
using RedRobin.DataAccess;
using RedRobin.DataAccess.Models;
using RedRobin.DataAccess.Models.Generated;

namespace UtilityRunner
{
    public class Program
    {
        private static void Main(string[] args)
        {
            CountUnmatchedXmlItemsWithExactNameMatchInDB();
        }

        private static void CountUnmatchedXmlItemsWithExactNameMatchInDB()
        {
            var db = new PetaPoco.Database("RedRobin");
            var db2 = new PetaPoco.Database("RedRobin");

            int i = 0;
            int found = 0;
            foreach (var menuitem in db.Query<XmlMenuItemsXref>("Select * from XmlMenuItemsXref where menuitemguid IS NULL"))
            {
                i++;
                Console.WriteLine("searching item " + menuitem.xmlitemname);
                var dalist = db2.Fetch<MenuItem>("select * from menuitems where replace(replace(replace(name, ',', ''),'/',''),' ','')=@0 ", menuitem.xmlitemname.Replace(",","").Replace("/","").Replace(" ",""));
                if (dalist.Count > 0)
                {
                    Console.WriteLine("*found menu item: " + dalist.First().name);
                    found++;
                }
            }
            Console.WriteLine(found);
            Console.ReadLine();

        }

        private static void MatchNewXmlToOldImport()
        {
            Console.WriteLine("starting");
            try
            {
                var xmlitems = XmlService.ParseXml(@"C:\root\dev\Spring Menu XML for Customizer 2.29.2012.xml");


                var db = new PetaPoco.Database("RedRobin");
                foreach (XmlMenuItem xmlMenuItem in xmlitems) //foreach item in the xml
                {
                    var mxr = new XmlMenuItemsXref();
                    mxr.xmlguid = new Guid(xmlMenuItem.ItemGuid);
                    mxr.xmlitemname = xmlMenuItem.ItemName;
                    Console.WriteLine("Xml Item: " + xmlMenuItem.ItemName);

                    var importmenuitem = GetMenuItemByName(xmlMenuItem.ItemName); //get the previous import item

                    if (importmenuitem != null) // new in xml or can't find mapping by name
                    {
                        MenuItem menuitem = null;
                        try
                        {
                            menuitem = db.Fetch<MenuItem>("select * from menuitems where DataObjectId=@0", importmenuitem.ItemGuid).SingleOrDefault();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        if (menuitem != null) // means the xmlimport and the database do not match
                        {
                            Console.WriteLine("Menu Item: " + menuitem.name);

                            mxr.menuitemguid = menuitem.DataObjectId;
                            mxr.menuitemname = menuitem.name;
                        }
                    }

                    mxr.Insert();

                    foreach (var xmlIngredient in xmlMenuItem.Ingredients)
                    {
                        Console.WriteLine("Xml Ingredient Item: " + xmlIngredient.ItemIngredientName);

                        var ixr = new XmlIngredientsXref();
                        ixr.xmlguid = new Guid(xmlIngredient.ItemIngredientGuid);
                        ixr.xmlingredientname = xmlIngredient.ItemIngredientName;

                        if (importmenuitem != null)
                        {
                            var db2 = new PetaPoco.Database("RedRobin");

                            var importingredientitem = GetIngredients(xmlIngredient.ItemIngredientName, importmenuitem.XmlImportMenuItemId); //get xml import ingredient
                            if (importingredientitem != null) //mean couldn't find this ingredient, deal with later
                            {
                                Ingredient ingredientitem = null;
                                try
                                {
                                    ingredientitem = db2.Fetch<Ingredient>("select * from ingredients where DataObjectId=@0", importingredientitem.ItemIngredientGuid).SingleOrDefault();
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }

                                if (ingredientitem != null) //means no match in database
                                {

                                    ixr.igredientname = ingredientitem.managename;
                                    ixr.ingredientsguid = ingredientitem.DataObjectId;
                                }

                            }
                        }

                        ixr.xmlmenuitemguid = new Guid(xmlMenuItem.ItemGuid);
                        ixr.xmlmenuitemname = xmlMenuItem.ItemName;
                        ixr.Insert();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        public static XmlImportIngredient GetIngredients(string name, int menuItemId)
        {
            var db = new PetaPoco.Database("RedRobin");
            try
            {
                return db.Fetch<XmlImportIngredient>("select * from XmlImportIngredients where ItemIngredientName=@0 and XmlImportMenuItemId=@1", name, menuItemId).SingleOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        public static XmlImportMenuItem GetMenuItemByName(string name)
        {
            var db = new PetaPoco.Database("RedRobin");
            try
            {
                return db.Fetch<XmlImportMenuItem>("select * from XmlImportMenuItems where itemname=@0", name).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            return null;
        }
    }
}

