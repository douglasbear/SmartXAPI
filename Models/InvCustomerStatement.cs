using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_CustomerStatement")]
    public partial class InvCustomerStatement
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ledgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(8000)]
        public string XRemarks { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PeriodID")]
        public int NPeriodId { get; set; }
        [Column("X_EntryForm")]
        [StringLength(27)]
        public string XEntryForm { get; set; }
        [Column("N_AccType")]
        public int NAccType { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Required]
        [Column("X_Bank")]
        [StringLength(1)]
        public string XBank { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(200)]
        public string XProjectName { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_VoucherDetailsID")]
        public int NVoucherDetailsId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("N_AgainstLedgerID")]
        public int? NAgainstLedgerId { get; set; }
        [Column("X_InvDescription")]
        [StringLength(2000)]
        public string XInvDescription { get; set; }
        [Column("X_CustPONo")]
        [StringLength(100)]
        public string XCustPono { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
    }
}
