using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace RedRobin.DataAccess.Models
{
    public class IngredientCandidates
    {
        public XmlIngredient XmlIngredient { get; set; }
        public List<IngredientAggregate> ByNameCandidates { get; set; }
        public List<IngredientAggregate> ByPreviousCandidates { get; set; }
        public KeyValuePair<string, string> AllIngredients { get; set; }  
    }
}
