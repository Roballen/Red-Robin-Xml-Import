using System;
using System.Collections.Generic;
using System.IO;
using PetaPoco;
using RedRobin.DataAccess.Models;
using RedRobin.DataAccess.Models.Generated;
using RedRobin.DataAccess.Utilities;
using System.Xml;
using Utilities;

namespace RedRobin.DataAccess
{

    public class XmlService
    {
        protected static XmlIngredient BuildIngredient(XmlNode ingredientNode)
        {
            XmlIngredient result = new XmlIngredient();

            XmlNode itemIngredientNameNode = ingredientNode.SelectSingleNode("item_ingredient_name");
            XmlNode itemIngredientGuidNode = ingredientNode.SelectSingleNode("item_ingredient_guid");
            XmlNode quantityNode = ingredientNode.SelectSingleNode("quantity");
            XmlNode quantityUnitNode = ingredientNode.SelectSingleNode("quantity_unit");
            XmlNode calsNode = ingredientNode.SelectSingleNode("cals");
            XmlNode fatCalsNode = ingredientNode.SelectSingleNode("fat_cals");
            XmlNode proteinNode = ingredientNode.SelectSingleNode("protein");
            XmlNode carbsNode = ingredientNode.SelectSingleNode("carbs");
            XmlNode fiberNode = ingredientNode.SelectSingleNode("fiber");
            XmlNode sugarNode = ingredientNode.SelectSingleNode("sugar");
            XmlNode fatNode = ingredientNode.SelectSingleNode("fat");
            XmlNode satFatNode = ingredientNode.SelectSingleNode("sat_fat");
            XmlNode transFatNode = ingredientNode.SelectSingleNode("trans_fat");
            XmlNode cholNode = ingredientNode.SelectSingleNode("chol");
            XmlNode sodNode = ingredientNode.SelectSingleNode("sod");
            result.ItemIngredientName = ValueUtility.ConvertNull(itemIngredientNameNode.InnerText, "");
            result.ItemIngredientGuid = ValueUtility.ConvertNull(itemIngredientGuidNode.InnerText, "");
            result.Quantity = ValueUtility.ConvertNull(quantityNode.InnerText, "");
            result.QuantityUnit = ValueUtility.ConvertNull(quantityUnitNode.InnerText, "");
            result.Calories = ValueUtility.ConvertNull(calsNode.InnerText, "");
            result.FatCalories = ValueUtility.ConvertNull(fatCalsNode.InnerText, "");
            result.Protein = ValueUtility.ConvertNull(proteinNode.InnerText, "");
            result.Carbohydrates = ValueUtility.ConvertNull(carbsNode.InnerText, "");
            result.Fiber = ValueUtility.ConvertNull(fiberNode.InnerText, "");
            result.Sugar = ValueUtility.ConvertNull(sugarNode.InnerText, "");
            result.Fat = ValueUtility.ConvertNull(fatNode.InnerText, "");
            result.SaturatedFat = ValueUtility.ConvertNull(satFatNode.InnerText, "");
            result.TransFat = ValueUtility.ConvertNull(transFatNode.InnerText, "");
            result.Cholesterol = ValueUtility.ConvertNull(cholNode.InnerText, "");
            result.Sodium = ValueUtility.ConvertNull(sodNode.InnerText, "");
            return result;
        }

        protected static XmlMenuItem BuildMenuItem(XmlNode menuItemNode)
        {
            XmlMenuItem result = new XmlMenuItem();

            XmlNode itemNameNode = menuItemNode.SelectSingleNode("item_name");
            XmlNode itemIdNode = menuItemNode.SelectSingleNode("item_id");
            XmlNode itemGuidNode = menuItemNode.SelectSingleNode("item_guid");
            XmlNode totalCalsNode = menuItemNode.SelectSingleNode("total_cals");
            XmlNode totalFatCalsNode = menuItemNode.SelectSingleNode("total_fat_cals");
            XmlNode totalProteinNode = menuItemNode.SelectSingleNode("total_protein");
            XmlNode totalCarbsNode = menuItemNode.SelectSingleNode("total_carbs");
            XmlNode totalFiberNode = menuItemNode.SelectSingleNode("total_fiber");
            XmlNode totalSugarNode = menuItemNode.SelectSingleNode("total_sugar");
            XmlNode totalFatNode = menuItemNode.SelectSingleNode("total_fat");
            XmlNode totalSatFatNode = menuItemNode.SelectSingleNode("total_sat_fat");
            XmlNode totalTransFatNode = menuItemNode.SelectSingleNode("total_trans_fat");
            XmlNode totalCholNode = menuItemNode.SelectSingleNode("total_chol");
            XmlNode totalSodNode = menuItemNode.SelectSingleNode("total_sod");
            result.ItemName = ValueUtility.ConvertNull(itemNameNode.InnerText, "");
            result.ItemId = ValueUtility.ConvertNull(itemIdNode.InnerText, "");
            result.ItemGuid = ValueUtility.ConvertNull(itemGuidNode.InnerText, "");
            result.TotalCalories = ValueUtility.ConvertNull(totalCalsNode.InnerText, "");
            result.TotalFatCalories = ValueUtility.ConvertNull(totalFatCalsNode.InnerText, "");
            result.TotalProtein = ValueUtility.ConvertNull(totalProteinNode.InnerText, "");
            result.TotalCarbohydrates = ValueUtility.ConvertNull(totalCarbsNode.InnerText, "");
            result.TotalFiber = ValueUtility.ConvertNull(totalFiberNode.InnerText, "");
            result.TotalSugar = ValueUtility.ConvertNull(totalSugarNode.InnerText, "");
            result.TotalFat = ValueUtility.ConvertNull(totalFatNode.InnerText, "");
            result.TotalSaturatedFat = ValueUtility.ConvertNull(totalSatFatNode.InnerText, "");
            result.TotalTransFat = ValueUtility.ConvertNull(totalTransFatNode.InnerText, "");
            result.TotalCholesterol = ValueUtility.ConvertNull(totalCholNode.InnerText, "");
            result.TotalSodium = ValueUtility.ConvertNull(totalSodNode.InnerText, "");
            return result;
        }

        /// <summary>
        /// this method is used for the backfilling parser
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static XmlMenuItems ParseXml(string filename)
        {
            if (!FileTools.SFileExists(filename))
                return new XmlMenuItems();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            return ParseXml(xmlDocument);

        }

        public static XmlMenuItems ParseXml(XmlDocument xmlDocument)
        {
            var result = new XmlMenuItems();
            XmlNodeList menuItemNodes = xmlDocument.SelectNodes("menu/menu_item");

            if (menuItemNodes == null)
                return result;

            foreach (XmlNode menuItemNode in menuItemNodes)
            {
                XmlMenuItem menuItem = BuildMenuItem(menuItemNode);
                if (menuItem.Ingredients == null)
                    menuItem.Ingredients = new List<XmlIngredient>();

                XmlNodeList ingredientNodes = menuItemNode.SelectNodes("menu_item_ingredient");
                if (ingredientNodes != null)
                {
                    foreach (XmlNode ingredientNode in ingredientNodes)
                    {
                        menuItem.Ingredients.Add(BuildIngredient(ingredientNode));
                    }
                }
                result.Add(menuItem);
            }

            return result;
        }

        public static XmlImportResults UploadXml(string filename, Stream xmlStream)
        {
 
            var result = new XmlImportResults();
            result.Success = false;
            DeletePreviousImports();


            var xmlDocument = new XmlDocument();

            xmlDocument.Load(xmlStream);
            var import = new XmlImport
                        {
                            ImportDate = DateTime.Now,
                            XmlFileName = filename,
                            XmlData = XmlUtility.ToString(xmlDocument)
                        };
            var importid =import.Insert();

            var menuitems = ParseXml(xmlDocument);

            foreach (var xmlMenuItem in menuitems)
            {
                xmlMenuItem.XmlImportId = Convert.ToInt32(importid);
                var menuid = xmlMenuItem.Insert();
                XmlMenuItemXrefRespositorycs.Save(xmlMenuItem);
                result.MenuItemsUploaded++;
                foreach (var ingredient in xmlMenuItem.Ingredients)
                {
                    ingredient.XmlImportMenuItemId = Convert.ToInt32(menuid);
                    ingredient.Save();
                    XmlIngredientsXrefRepository.Save(ingredient, xmlMenuItem.ItemGuid,xmlMenuItem.ItemName);
                    result.IngredientsUploaded++;
                }
            }

            result.Success = true;
            return result;
        }



        private static void DeletePreviousImports()
        {
            var db = new Database("RedRobin");
            // have to delete the suckas in order to maintain referential integrity
            db.Execute("delete from xmlimportingredients");
            db.Execute("delete from xmlimportmenuitems");
            db.Execute("Delete from xmlimports");

        }

    }
}

