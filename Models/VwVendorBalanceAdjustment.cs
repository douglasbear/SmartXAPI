using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVendorBalanceAdjustment
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("Adjustment Date")]
        [StringLength(8000)]
        public string AdjustmentDate { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_PartyType")]
        public int? NPartyType { get; set; }
        [Column("N_TransType")]
        public int? NTransType { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [StringLength(30)]
        public string Netamt { get; set; }
    }
}
