using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvLocationStock
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("Location Code")]
        public string LocationCode { get; set; }
        [Column("Location Name")]
        public string LocationName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("Branch Name")]
        [StringLength(50)]
        public string BranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        public bool? Active { get; set; }
        [Column("B_ShowAllData")]
        public bool? BShowAllData { get; set; }
        public bool? IsCurrent { get; set; }
        [Column("B_IsDefault")]
        public bool? BIsDefault { get; set; }
        [Column("N_CurrentStock")]
        [StringLength(50)]
        public string NCurrentStock { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Required]
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [StringLength(500)]
        public string Expr1 { get; set; }
    }
}
