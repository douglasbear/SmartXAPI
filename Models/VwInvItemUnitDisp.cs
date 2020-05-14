using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemUnitDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Column("Unit Code")]
        [StringLength(500)]
        public string UnitCode { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("Base Unit")]
        [StringLength(500)]
        public string BaseUnit { get; set; }
        public double? Quantity { get; set; }
        [Column("B_BaseUnit")]
        public bool? BBaseUnit { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [StringLength(30)]
        public string ItemCode { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
    }
}
