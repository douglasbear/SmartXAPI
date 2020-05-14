using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_CostCentreTransactions")]
    public partial class AccCostCentreTransactions
    {
        [Key]
        [Column("N_CostCenterTransID")]
        public int NCostCenterTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int NVoucherDetailsId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Narration")]
        [StringLength(250)]
        public string XNarration { get; set; }
        [Column("X_Naration")]
        [StringLength(250)]
        public string XNaration { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_EmployeeID")]
        public int? NEmployeeId { get; set; }
    }
}
