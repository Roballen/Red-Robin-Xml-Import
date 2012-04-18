using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedRobin.DataAccess.Models
{
    public class XmlImportResults
    {
        public XmlImportResults()
        {
            MenuItemsUploaded = 0;
            IngredientsUploaded = 0;
        }

        public bool Success { get; set; }
        public int MenuItemsUploaded { get; set; }
        public int IngredientsUploaded { get; set; }
        public string ErrorMessage { get; set; }
    }
}
