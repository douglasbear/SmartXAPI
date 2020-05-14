using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_CustomerProjects")]
    public partial class InvCustomerProjects
    {
        [Key]
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
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
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_EstimateCost", TypeName = "money")]
        public decimal? NEstimateCost { get; set; }
        [Column("B_IsClose")]
        public bool? BIsClose { get; set; }
        [Column("D_CloseDate", TypeName = "datetime")]
        public DateTime? DCloseDate { get; set; }
        [Column("N_ContractAmt", TypeName = "money")]
        public decimal? NContractAmt { get; set; }
        [Column("N_Progress")]
        public int? NProgress { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_LocationName")]
        [StringLength(2000)]
        public string XLocationName { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("D_ActualEndDate", TypeName = "date")]
        public DateTime? DActualEndDate { get; set; }
        [Column("N_Typeval")]
        public int? NTypeval { get; set; }
        [Column("N_Permit")]
        public int? NPermit { get; set; }
        [Column("N_Team")]
        public int? NTeam { get; set; }
        [Column("X_Customers")]
        [StringLength(2000)]
        public string XCustomers { get; set; }
        [Column("X_Suppliers")]
        [StringLength(2000)]
        public string XSuppliers { get; set; }
        [Column("N_CostOfSalesLedgerID")]
        public int? NCostOfSalesLedgerId { get; set; }
        [Column("N_SalesLedgerID")]
        public int? NSalesLedgerId { get; set; }
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
        [Column("N_DesignCost", TypeName = "money")]
        public decimal? NDesignCost { get; set; }
        [Column("N_TenderID")]
        public int? NTenderId { get; set; }
        [Column("X_EmpsID")]
        [StringLength(100)]
        public string XEmpsId { get; set; }
        [Column("X_SuppsID")]
        [StringLength(100)]
        public string XSuppsId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
        [Column("X_MainProject")]
        [StringLength(500)]
        public string XMainProject { get; set; }
        [Column("N_MainProjectID")]
        public int? NMainProjectId { get; set; }
        [Column("N_DefaultInvGrp")]
        public int? NDefaultInvGrp { get; set; }
        [Column("N_DefaultMappingID")]
        public int? NDefaultMappingId { get; set; }
        [Column("X_ContractNo")]
        [StringLength(100)]
        public string XContractNo { get; set; }
        [Column("X_PO")]
        [StringLength(500)]
        public string XPo { get; set; }
    }
}
