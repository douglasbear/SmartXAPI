using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeDetailsForPosting
    {
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("B_Posted")]
        public bool? BPosted { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_PayType")]
        public int? NPayType { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("X_PayElementName")]
        [StringLength(100)]
        public string XPayElementName { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_LoanLedgerID")]
        public int? NLoanLedgerId { get; set; }
        [Column("N_SalaryPayMethod")]
        public int? NSalaryPayMethod { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("B_ISGOSI")]
        public int BIsgosi { get; set; }
        [Column("Empl_Branch")]
        public int? EmplBranch { get; set; }
        [Column("N_CostCentreID")]
        public int NCostCentreId { get; set; }
    }
}
