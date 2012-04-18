using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace RedRobin.DataAccess.Models
{
    public class UpdateObject
    {
        [Column] //not null
        public Guid xmlmenuguid { get; set; }

        [Column]
        public Guid? menuguid { get; set; }

        [Column]
        public Guid xmlingredientguid { get; set; }

        [Column]
        public Guid? ingredientguid { get; set; }

        [Column]
        public int ingredientid { get; set; }

        [Column]
        public int menuid { get; set; }

        [Column]
        public int sortindex { get; set; }

        [Column]
        public int xrefpk { get; set; }

        [Column]
        public Guid? DataObjectId { get; set; }

        [Column]
        public Guid? ServiceObjectId { get; set; }

    }
}
