using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeeRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("X_PositionCode")]
        [StringLength(50)]
        public string XPositionCode { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_ReportToID")]
        public int? NReportToId { get; set; }
        [Column("X_SupervisorCode")]
        [StringLength(50)]
        public string XSupervisorCode { get; set; }
        [Column("X_Supervisor")]
        [StringLength(100)]
        public string XSupervisor { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Sex")]
        [StringLength(50)]
        public string XSex { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_SocSecNo")]
        [StringLength(50)]
        public string XSocSecNo { get; set; }
        [Column("D_HireDate", TypeName = "datetime")]
        public DateTime? DHireDate { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_City")]
        [StringLength(50)]
        public string XCity { get; set; }
        [Column("X_State")]
        [StringLength(50)]
        public string XState { get; set; }
        [Column("X_ZipCode")]
        [StringLength(50)]
        public string XZipCode { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("X_Phone1")]
        [StringLength(50)]
        public string XPhone1 { get; set; }
        [Column("X_Phone2")]
        [StringLength(50)]
        public string XPhone2 { get; set; }
        [Column("X_EmailID")]
        [StringLength(100)]
        public string XEmailId { get; set; }
        [Column("X_NickName")]
        [StringLength(50)]
        public string XNickName { get; set; }
        [Column("X_AlternateName")]
        [StringLength(50)]
        public string XAlternateName { get; set; }
        [Column("X_PassportNo")]
        [StringLength(50)]
        public string XPassportNo { get; set; }
        [Column("D_PassportExpiry", TypeName = "datetime")]
        public DateTime? DPassportExpiry { get; set; }
        [Column("D_PassportIssue", TypeName = "datetime")]
        public DateTime? DPassportIssue { get; set; }
        [Column("X_IqamaNo")]
        [StringLength(50)]
        public string XIqamaNo { get; set; }
        [Column("D_IqamaExpiry", TypeName = "datetime")]
        public DateTime? DIqamaExpiry { get; set; }
        [Column("X_MaritalStatus")]
        [StringLength(50)]
        public string XMaritalStatus { get; set; }
        [Column("X_SName")]
        [StringLength(50)]
        public string XSname { get; set; }
        [Column("X_SContactNo")]
        [StringLength(50)]
        public string XScontactNo { get; set; }
        [Column("X_SPassportNo")]
        [StringLength(50)]
        public string XSpassportNo { get; set; }
        [Column("D_SPassportExpiry", TypeName = "datetime")]
        public DateTime? DSpassportExpiry { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("B_ContractEmployee")]
        public bool? BContractEmployee { get; set; }
        [Column("X_EmployeeSponsor")]
        [StringLength(100)]
        public string XEmployeeSponsor { get; set; }
        [Column("X_EmployeeSponsorNo")]
        [StringLength(40)]
        public string XEmployeeSponsorNo { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("B_AlertPassport")]
        public bool? BAlertPassport { get; set; }
        [Column("B_AlertIqama")]
        public bool? BAlertIqama { get; set; }
        [Column("B_AlertSPassport")]
        public bool? BAlertSpassport { get; set; }
        [Column("N_NoofVacationDays")]
        public int? NNoofVacationDays { get; set; }
        [Column("X_EmpNameLocale")]
        [StringLength(100)]
        public string XEmpNameLocale { get; set; }
        [Column("X_NationalityLocale")]
        [StringLength(100)]
        public string XNationalityLocale { get; set; }
        [Column("X_EmpImageNAme")]
        [StringLength(100)]
        public string XEmpImageName { get; set; }
        [Column("D_SPassportIssue", TypeName = "datetime")]
        public DateTime? DSpassportIssue { get; set; }
        [Column("N_NationalityID")]
        public int? NNationalityId { get; set; }
        [Column("B_EnablePortalLogin")]
        public bool? BEnablePortalLogin { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("X_BankAccountNo")]
        [StringLength(50)]
        public string XBankAccountNo { get; set; }
        [Column("N_SalaryPayMethod")]
        public int? NSalaryPayMethod { get; set; }
        [Column("X_PassportFileName")]
        [StringLength(50)]
        public string XPassportFileName { get; set; }
        [Column("X_IqamaFileName")]
        [StringLength(50)]
        public string XIqamaFileName { get; set; }
        [Column("X_SpousePassportFileName")]
        [StringLength(50)]
        public string XSpousePassportFileName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        public string ImageName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("D_StatusDate", TypeName = "date")]
        public DateTime? DStatusDate { get; set; }
        public int? Expr1 { get; set; }
        [Column("X_InsuranceNo")]
        [StringLength(100)]
        public string XInsuranceNo { get; set; }
        [Column("X_IqamaProfession")]
        [StringLength(50)]
        public string XIqamaProfession { get; set; }
    }
}
