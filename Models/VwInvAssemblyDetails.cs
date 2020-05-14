using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvAssemblyDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AssemblyID")]
        public int? NAssemblyId { get; set; }
        [Column("N_AssemblyDetailsID")]
        public int NAssemblyDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Qtyused")]
        public double? NQtyused { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_Stock")]
        public double? NStock { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_AlternativeItemID")]
        public int? NAlternativeItemId { get; set; }
        [Column("X_AlterProductCode")]
        [StringLength(100)]
        public string XAlterProductCode { get; set; }
        [Column("X_AlterProductName")]
        [StringLength(800)]
        public string XAlterProductName { get; set; }
        [Column("N_RecycleQty")]
        public double? NRecycleQty { get; set; }
        [Column("N_WastageQty")]
        public double? NWastageQty { get; set; }
        [Column("N_AltMainItemID")]
        public double? NAltMainItemId { get; set; }
    }
}
