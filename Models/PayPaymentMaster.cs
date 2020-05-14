using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PaymentMaster")]
    public partial class PayPaymentMaster
    {
        public PayPaymentMaster()
        {
            PayPaymentDetails = new HashSet<PayPaymentDetails>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
        [Column("D_CreatedDate", TypeName = "datetime")]
        public DateTime? DCreatedDate { get; set; }
        [Column("D_ModifiedDate", TypeName = "datetime")]
        public DateTime? DModifiedDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("D_SalFromDate", TypeName = "datetime")]
        public DateTime? DSalFromDate { get; set; }
        [Column("D_SalToDate", TypeName = "datetime")]
        public DateTime? DSalToDate { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("X_BatchRemarks")]
        [StringLength(1000)]
        public string XBatchRemarks { get; set; }

        [InverseProperty("NTrans")]
        public virtual ICollection<PayPaymentDetails> PayPaymentDetails { get; set; }
    }
}
