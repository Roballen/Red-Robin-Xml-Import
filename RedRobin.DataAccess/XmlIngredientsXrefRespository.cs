using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using RedRobin.DataAccess.Models.Generated;

namespace RedRobin.DataAccess
{
    public class XmlIngredientsXrefRepository
    {
        public static void Save(XmlImportIngredient ingredient, string menuitemguid, string menuitemname)
        {
            var db = new Database("RedRobin");

            var records = db.Fetch<XmlIngredientsXref>(" where xmlguid=@0 and xmlmenuitemguid=@1", ingredient.ItemIngredientGuid, menuitemguid);

            if (records.Count >= 1) return;
            var insert = new XmlIngredientsXref();
            insert.xmlguid = new Guid(ingredient.ItemIngredientGuid);
            insert.xmlingredientname = ingredient.ItemIngredientName;
            insert.xmlmenuitemguid = new Guid(menuitemguid);
            insert.xmlmenuitemname = menuitemname;
            insert.Save();
        }


    }
}
