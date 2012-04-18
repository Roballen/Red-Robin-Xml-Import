using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedRobin.DataAccess.Models
{
    public class UpdateResults
    {
        public UpdateResults()
        {
            Errors = new Dictionary<string, string>();
            MenuItemsUpdated = 0;
            IngredientsUpdated = 0;
            NutrionalFactsUpdates = 0;
        }

        public Dictionary<string, string> Errors { get; set; }
        public int MenuItemsUpdated { get; set; }
        public int IngredientsUpdated { get; set; }
        public int NutrionalFactsUpdates { get; set; }
    }
}
