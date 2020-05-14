using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayProcessedDetailsCsv
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_EmpPatakaName")]
        [StringLength(100)]
        public string XEmpPatakaName { get; set; }
        [Column("X_BankAccountNo")]
        [StringLength(50)]
        public string XBankAccountNo { get; set; }
        [Column("X_BankCode")]
        [StringLength(50)]
        public string XBankCode { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Required]
        [Column("X_PaymentDescription")]
        [StringLength(1)]
        public string XPaymentDescription { get; set; }
        [Required]
        [Column("X_ReturnCode")]
        [StringLength(1)]
        public string XReturnCode { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Required]
        [Column("X_Transactionnumber")]
        [StringLength(1)]
        public string XTransactionnumber { get; set; }
        [Required]
        [Column("X_Transactionstatus")]
        [StringLength(1)]
        public string XTransactionstatus { get; set; }
        [Column("X_TransDate")]
        [StringLength(8)]
        public string XTransDate { get; set; }
        [Column("N_SalaryPayMethod")]
        public int? NSalaryPayMethod { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_BasicSalary", TypeName = "money")]
        public decimal? NBasicSalary { get; set; }
        [Column("N_HA", TypeName = "money")]
        public decimal? NHa { get; set; }
        [Column("N_OtherEarnings", TypeName = "money")]
        public decimal? NOtherEarnings { get; set; }
        [Column("N_OtherDeductions", TypeName = "money")]
        public decimal? NOtherDeductions { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
        [Required]
        [Column("N_EntryFrom")]
        [StringLength(3)]
        public string NEntryFrom { get; set; }
        [Required]
        [Column("N_PaymentID")]
        [StringLength(1)]
        public string NPaymentId { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(1)]
        public string XDescription { get; set; }
        [Required]
        [Column("N_PayTypeID")]
        [StringLength(1)]
        public string NPayTypeId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Type")]
        public int NType { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("N_EmpTypeID")]
        public int? NEmpTypeId { get; set; }
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
    }
}
