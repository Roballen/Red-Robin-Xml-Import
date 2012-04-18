using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using RedRobin.DataAccess.Models.Generated;

namespace RedRobin.DataAccess
{
    public class XmlMenuItemXrefRespositorycs
    {
        public static void Save(XmlImportMenuItem menuitem)
        {
            var db = new Database("RedRobin");
            var records = db.Fetch<XmlMenuItemsXref>(" where xmlguid=@0", menuitem.ItemGuid);

            if (records.Count >= 1) return;

            var xref = new XmlMenuItemsXref();
            xref.xmlguid = new Guid(menuitem.ItemGuid);
            xref.xmlitemname = menuitem.ItemName;
            xref.Save();
        }
    }
}
