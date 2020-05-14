using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPomrnPending
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("D_POrderDate", TypeName = "datetime")]
        public DateTime? DPorderDate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("Mrn_QtyToStock")]
        public double? MrnQtyToStock { get; set; }
        [Column("Mrn_ReturnQty")]
        public double? MrnReturnQty { get; set; }
        [Column("B_IsDirectMRN")]
        public bool? BIsDirectMrn { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
    }
}
