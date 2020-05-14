using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_VoucherMaster")]
    public partial class AccVoucherMaster
    {
        public AccVoucherMaster()
        {
            AccVoucherMasterDetails = new HashSet<AccVoucherMasterDetails>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PeriodID")]
        public int? NPeriodId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
        [Column("X_EntryFrom")]
        [StringLength(100)]
        public string XEntryFrom { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_DefLedgerID")]
        public int? NDefLedgerId { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_DefLedgerType")]
        [StringLength(50)]
        public string XDefLedgerType { get; set; }
        [Column("B_IsAccPosted")]
        public bool? BIsAccPosted { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("N_RequestID")]
        public int? NRequestId { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.AccVoucherMaster))]
        public virtual AccCompany NCompany { get; set; }
        [InverseProperty("NVoucher")]
        public virtual ICollection<AccVoucherMasterDetails> AccVoucherMasterDetails { get; set; }
    }
}
