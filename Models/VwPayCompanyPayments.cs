using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayCompanyPayments
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
        [Column("N_PositionID")]
        public int? NPositionId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_ReportToID")]
        public int? NReportToId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
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
        [Column("D_PassportIssue", TypeName = "datetime")]
        public DateTime? DPassportIssue { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
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
        [Column("N_NationalityID")]
        public int? NNationalityId { get; set; }
        [Column("X_EmpImageNAme")]
        [StringLength(100)]
        public string XEmpImageName { get; set; }
        [Column("D_SPassportIssue", TypeName = "datetime")]
        public DateTime? DSpassportIssue { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("N_LoanLedgerID")]
        public int? NLoanLedgerId { get; set; }
        [Required]
        [StringLength(1)]
        public string Signature { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("B_Posted")]
        public int? BPosted { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("X_Batch")]
        [StringLength(100)]
        public string XBatch { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("D_CreatedDate", TypeName = "datetime")]
        public DateTime? DCreatedDate { get; set; }
        public int? Expr1 { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Required]
        [Column("N_PayType")]
        [StringLength(10)]
        public string NPayType { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
