using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedRobin.DataAccess.Models.Generated;
using Utilities;

namespace RedRobin.DataAccess.Models
{
    public class MenuItemAggregate : MenuItem
    {
        public MenuItemAggregate()
        {
            Ingredients = new List<IngredientAggregate>();
        }
        public List<IngredientAggregate> Ingredients { get; set; } 
    }

    public class IngredientAggregate : Ingredient
    {
        public IngredientAggregate()
        {
            Nutrients = new List<NutrientDisplay>();
        }
        public List<NutrientDisplay> Nutrients { get; set; } 
    }

    public class NutrientDisplay : NutritionFact
    {
        public string NutrientInfo
        {
            get
            {
                if (value.Trim() != "0" || value.IsEmpty())
                    return GetNutrientTypeString(nutritionfacttypeid) + ": " + value + GetUnitOfMeasure(nutritionfacttypeid);
                else
                    return "";
            }
        }

        private string GetNutrientTypeString(int type)
        {
            switch (type)
            {
                case 1:
                    return "Calories";
                case 3:
                    return "Weight";
                case 4:
                    return "Total Fat";
                case 5:
                    return "Saturated Fat";
                case 6:
                    return "Sodium";
                case 7:
                    return "Carbohydrates";
                case 8:
                    return "Protein";
                case 9:
                    return "Sugars";
                case 10:
                    return "Cholesterol";
                case 11:
                    return "Calories From Fat";
                case 12:
                    return "Dietary Fiber";
                case 13:
                    return "Trans fat";
                default:
                    return "Unknown";
            }
        }

        private string GetUnitOfMeasure(int type)
        {
            switch (type)
            {
                case 6:
                case 10:
                    return "mg";
                case 1:
                case 11:
                case 14:
                    return "";
                default:
                    return "g";

            }
        }
    }

//    public class IngredietMatch
//    {
//        public IngredietMatch()
//        {
//            MatchedIngredient = new IngredientAggregate();
//            XmlIngredient = new XmlIngredient();
//        }
//
//        public IngredientAggregate MatchedIngredient { get; set; }
//        public XmlIngredient XmlIngredient { get; set; }
//    }
//
//    public class MenuMatch
//    {
//        public MenuMatch()
//        {
//            MatchedMenuItem = new MenuItemAggregate();
//            XmlMenuItem = new XmlMenuItem();
//        }
//        public MenuItemAggregate MatchedMenuItem { get; set; }
//        public XmlMenuItem XmlMenuItem { get; set; }
//    }

}
