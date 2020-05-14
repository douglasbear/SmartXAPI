using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrsPoMrnStatus
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("N_PRSDetailsID")]
        public int NPrsdetailsId { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("N_POrderDetailsID")]
        public int? NPorderDetailsId { get; set; }
        [Column("N_MRNID")]
        public int? NMrnid { get; set; }
        [Column("N_MRNDetailsID")]
        public int? NMrndetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("PRS_N_Cose", TypeName = "money")]
        public decimal? PrsNCose { get; set; }
        [Column("N_BinBalance")]
        public double? NBinBalance { get; set; }
        [Column("N_DepQty")]
        public double? NDepQty { get; set; }
        [Column("PRS_N_Qty")]
        public double? PrsNQty { get; set; }
        [Column("PO_N_Qty")]
        public double? PoNQty { get; set; }
        [Column("MRN_Qty")]
        public double? MrnQty { get; set; }
        [Column("MRN_ReturnQty")]
        public int? MrnReturnQty { get; set; }
        [Required]
        [StringLength(3)]
        public string Status { get; set; }
        [Column("BalancPRS")]
        public double? BalancPrs { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
    }
}
