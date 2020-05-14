using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemCost
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_ItemCost")]
        public double? NItemCost { get; set; }
        [Column("N_QtyOnHand")]
        public double? NQtyOnHand { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
    }
}
