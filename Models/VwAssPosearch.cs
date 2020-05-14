using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssPosearch
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
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_TotAmt")]
        [StringLength(30)]
        public string NTotAmt { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_POType")]
        public int NPotype { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_Processed")]
        public bool NProcessed { get; set; }
    }
}
