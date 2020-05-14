using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectTransferItemDetails
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_ItemCost", TypeName = "decimal(20, 6)")]
        public decimal? NItemCost { get; set; }
        [Column("N_CurrentStock")]
        public double? NCurrentStock { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("N_LPrice", TypeName = "decimal(20, 6)")]
        public decimal? NLprice { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
