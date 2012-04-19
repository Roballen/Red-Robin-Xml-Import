using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using RedRobin.DataAccess.Models.Generated;

namespace RedRobin.DataAccess.Models
{
    public class XmlIngredient : XmlImportIngredient
    {
        public string XmlGuid { get; set; }
        public IngredientAggregate IngredientMatch { get; set; }
    }

    public class DuplicateManager
    {
        public DuplicateManager()
        {
            Duplicates = new Dictionary<Duplicate, List<IngredientAggregate>>();
        }
        public Dictionary<Duplicate, List<IngredientAggregate>> Duplicates { get; set; }
    }

    public class Duplicate
    {
        public string DisplayName { get; set; }
        public string Calories { get; set; }
        public string Quantity { get; set; }
        public string Name { get; set; }
        public int NumberOfDups { get; set; }
    }

    public class XmlMenuItem : XmlImportMenuItem
    {
        public XmlMenuItem()
        {
            Ingredients = new List<XmlIngredient>();
        }
        public string XmlGuid { get; set; }
        public List<XmlIngredient> Ingredients { get; set; }
        public MenuItemAggregate MenuItemMatch { get; set; }
    }

    public class XmlMenuItems : List<XmlMenuItem>{}

    public class DuplicateIngredients : XmlImportIngredient
    {
        [Column]
        public int NumOccurrences { get; set; }
    }



}


