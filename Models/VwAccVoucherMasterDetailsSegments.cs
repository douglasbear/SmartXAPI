using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccVoucherMasterDetailsSegments
    {
        [Column("N_CostCenterTransID")]
        public int NCostCenterTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_VoucherID")]
        public int? NVoucherId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int? NVoucherDetailsId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Naration")]
        [StringLength(250)]
        public string XNaration { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
    }
}
