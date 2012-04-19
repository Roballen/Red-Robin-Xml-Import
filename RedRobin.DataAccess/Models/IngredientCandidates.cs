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
        public List<IngredientCandidate> ByNameCandidates { get; set; }
        public List<IngredientCandidate> ByPreviousCandidates { get; set; }
        public KeyValuePair<string, string> AllIngredients { get; set; }  
    }
}
