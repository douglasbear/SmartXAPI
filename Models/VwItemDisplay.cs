using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemDisplay
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Item Code")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [Column("Product Code")]
        [StringLength(100)]
        public string ProductCode { get; set; }
        [StringLength(800)]
        public string Description { get; set; }
        [Column("Part No")]
        [StringLength(250)]
        public string PartNo { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        [Column("Item Class")]
        [StringLength(25)]
        public string ItemClass { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_WarehouseID")]
        public int? NWarehouseId { get; set; }
        [Column("N_ItemTypeID")]
        public int NItemTypeId { get; set; }
    }
}
