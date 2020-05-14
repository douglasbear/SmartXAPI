using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayAmountDetailsForPay
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_PayrunText")]
        [StringLength(100)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public int? BPostedAccount { get; set; }
        [Required]
        [Column("N_EntryFrom")]
        [StringLength(3)]
        public string NEntryFrom { get; set; }
        [Column("N_PaymentID")]
        public int? NPaymentId { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayTypeID")]
        public int NPayTypeId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Type")]
        public int NType { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
    }
}
