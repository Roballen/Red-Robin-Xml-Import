using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedRobin.DataAccess.Models;
using RedRobin.DataAccess.Models.Generated;

namespace RedRobin.DataAccess
{
    public interface IMatchRepository
    {
        List<XmlMenuItemsXref> GetMatchedMenuItems();
        List<XmlMenuItemsXref> GetUnMatchedMenuItems();

        List<XmlIngredientsXref> GetMatchedIngredients();
        List<XmlIngredientsXref> GetUnMatchedIngredients();

        List<Ingredient> GetMatchedIngredients(String xmlguid);
        List<XmlIngredientsXref> GetUnMatchedIngredients(string xmlguid);
    }
}
