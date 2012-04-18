using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedRobin.DataAccess.Models.Generated;

namespace RedRobin.DataAccess.Models
{
    public class MenuItemCandidates
    {
        public XmlMenuItem XmlMenuItem { get; set; }
        public List<MenuItem> ByNameCandidates { get; set; }
        //public List<MenuItem> ByPreviousCandidates { get; set; } //not applicable
        public KeyValuePair<string, string> AllMenuItems { get; set; } //hopefully never have to use
    }
}
