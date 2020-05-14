using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvProjectDashBoard
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectDescription")]
        [StringLength(250)]
        public string XProjectDescription { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Required]
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Address")]
        [StringLength(500)]
        public string XAddress { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("D_StartDate")]
        [StringLength(8000)]
        public string DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_EstimateCost")]
        [StringLength(30)]
        public string NEstimateCost { get; set; }
        [Column("B_IsClose")]
        public bool? BIsClose { get; set; }
        [Column("N_ContractAmt")]
        [StringLength(30)]
        public string NContractAmt { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Progress")]
        [StringLength(200)]
        public string NProgress { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Required]
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Required]
        [Column("X_LedgerName")]
        [StringLength(500)]
        public string XLedgerName { get; set; }
        [Column("D_ActualEndDate", TypeName = "date")]
        public DateTime? DActualEndDate { get; set; }
        [Column("N_Typeval")]
        public int? NTypeval { get; set; }
        [Column("N_Permit")]
        public int? NPermit { get; set; }
        [Column("N_Team")]
        public int? NTeam { get; set; }
        [Column("X_Suppliers")]
        [StringLength(2000)]
        public string XSuppliers { get; set; }
        [Column("X_Employee")]
        [StringLength(2000)]
        public string XEmployee { get; set; }
        [Column("D_PermitStartDate", TypeName = "date")]
        public DateTime? DPermitStartDate { get; set; }
        [Column("D_PermitEndDate", TypeName = "date")]
        public DateTime? DPermitEndDate { get; set; }
        [Column("D_PermitActualEndDate", TypeName = "date")]
        public DateTime? DPermitActualEndDate { get; set; }
        [Column("X_District")]
        [StringLength(100)]
        public string XDistrict { get; set; }
        [Column("N_DesignCost")]
        [StringLength(30)]
        public string NDesignCost { get; set; }
        [Column("X_LocationName")]
        [StringLength(2000)]
        public string XLocationName { get; set; }
        [Column("Expense Account")]
        [StringLength(500)]
        public string ExpenseAccount { get; set; }
        [Column("Income Account")]
        [StringLength(500)]
        public string IncomeAccount { get; set; }
        [Column("Income Code")]
        [StringLength(50)]
        public string IncomeCode { get; set; }
        [Column("Expense Code")]
        [StringLength(50)]
        public string ExpenseCode { get; set; }
        [Column("N_TenderID")]
        public int? NTenderId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [StringLength(30)]
        public string ActualBudget { get; set; }
        [StringLength(30)]
        public string RemainingBudget { get; set; }
        public double? AwardedBudget { get; set; }
        [StringLength(30)]
        public string CommittedBudget { get; set; }
        [Column("X_PO")]
        [StringLength(500)]
        public string XPo { get; set; }
        [Column("N_MainProjectID")]
        public int NMainProjectId { get; set; }
    }
}
