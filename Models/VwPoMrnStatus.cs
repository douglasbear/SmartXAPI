using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPoMrnStatus
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
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("PO_N_Qty")]
        public double? PoNQty { get; set; }
        [Column("MRN_QtyToStock")]
        public int? MrnQtyToStock { get; set; }
        [Column("MRN_ReturnQty")]
        public int? MrnReturnQty { get; set; }
    }
}
