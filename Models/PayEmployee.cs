using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_Employee")]
    public partial class PayEmployee
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("X_BankAccountNo")]
        [StringLength(50)]
        public string XBankAccountNo { get; set; }
        [Column("N_NationalityID")]
        public int? NNationalityId { get; set; }
        [Column("X_EmpImageNAme")]
        [StringLength(100)]
        public string XEmpImageName { get; set; }
        [Column("D_SPassportIssue", TypeName = "datetime")]
        public DateTime? DSpassportIssue { get; set; }
        public string ImageName { get; set; }
        [Column("N_LoanLedgerID")]
        public int? NLoanLedgerId { get; set; }
        [Column("B_EnablePortalLogin")]
        public bool? BEnablePortalLogin { get; set; }
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
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_GOSIStart", TypeName = "datetime")]
        public DateTime? NGosistart { get; set; }
        [Column("N_GOSIEnd", TypeName = "datetime")]
        public DateTime? NGosiend { get; set; }
        [Column("N_GOSIPayID")]
        public int? NGosipayId { get; set; }
        [Column("N_Empcontribution", TypeName = "money")]
        public decimal? NEmpcontribution { get; set; }
        [Column("N_Compcontribution", TypeName = "money")]
        public decimal? NCompcontribution { get; set; }
        [Column("N_EmpTypeID")]
        public int? NEmpTypeId { get; set; }
        [Column("B_EnableSalesexecutive")]
        public bool? BEnableSalesexecutive { get; set; }
        [Column("B_EnableApplicationLogin")]
        public bool? BEnableApplicationLogin { get; set; }
        [Column("I_Employe_Image", TypeName = "image")]
        public byte[] IEmployeImage { get; set; }
        [Column("N_CatagoryId")]
        public int? NCatagoryId { get; set; }
        [Column("X_Email2")]
        [StringLength(100)]
        public string XEmail2 { get; set; }
        [Column("X_EmrgncyContact")]
        [StringLength(50)]
        public string XEmrgncyContact { get; set; }
        [Column("X_IqamaRefName")]
        [StringLength(100)]
        public string XIqamaRefName { get; set; }
        [Column("X_PassportRefName")]
        [StringLength(100)]
        public string XPassportRefName { get; set; }
        [Column("B_EnableTeacher")]
        public bool? BEnableTeacher { get; set; }
        [Column("X_DefEmpCode")]
        [StringLength(50)]
        public string XDefEmpCode { get; set; }
        [Column("X_EducationQual")]
        [StringLength(50)]
        public string XEducationQual { get; set; }
        [Column("X_Eligibility")]
        [StringLength(50)]
        public string XEligibility { get; set; }
        [Column("X_InsuranceNo")]
        [StringLength(100)]
        public string XInsuranceNo { get; set; }
        [Column("N_InsuranceID")]
        public int? NInsuranceId { get; set; }
        [Column("D_InsStart", TypeName = "datetime")]
        public DateTime? DInsStart { get; set; }
        [Column("D_InsExpiry", TypeName = "datetime")]
        public DateTime? DInsExpiry { get; set; }
        [Column("X_EmpInsFile")]
        [StringLength(50)]
        public string XEmpInsFile { get; set; }
        [Column("X_EmpInsRefFile")]
        [StringLength(50)]
        public string XEmpInsRefFile { get; set; }
        [Column("B_AlertInsurance")]
        public bool? BAlertInsurance { get; set; }
        [Column("X_EmpPatakaName")]
        [StringLength(100)]
        public string XEmpPatakaName { get; set; }
        [Column("D_StatusDate", TypeName = "date")]
        public DateTime? DStatusDate { get; set; }
        [Column("X_EmergencyNum")]
        [StringLength(500)]
        public string XEmergencyNum { get; set; }
        [Column("N_InsCount")]
        public int? NInsCount { get; set; }
        [Column("X_SCEnumber")]
        [StringLength(50)]
        public string XScenumber { get; set; }
        [Column("D_SCEdate", TypeName = "datetime")]
        public DateTime? DScedate { get; set; }
        [Column("N_HierarchyID")]
        public int? NHierarchyId { get; set; }
        [Column("N_ApprovalID")]
        public int? NApprovalId { get; set; }
        [Column("N_SalaryPeriodAdjustment")]
        public int? NSalaryPeriodAdjustment { get; set; }
        [Column("X_TicketType")]
        [StringLength(50)]
        public string XTicketType { get; set; }
        [Column("N_TicketAmount", TypeName = "money")]
        public decimal? NTicketAmount { get; set; }
        [Column("N_ContractInMonths")]
        public int? NContractInMonths { get; set; }
        [Column("X_IqamaProfession")]
        [StringLength(50)]
        public string XIqamaProfession { get; set; }
        [Column("N_RoomID")]
        public int? NRoomId { get; set; }
        [Column("N_AccStartDate", TypeName = "datetime")]
        public DateTime? NAccStartDate { get; set; }
        [Column("N_AccEndDate", TypeName = "datetime")]
        public DateTime? NAccEndDate { get; set; }
        [Column("N_AccID")]
        public int? NAccId { get; set; }
        [Column("X_TicketRoute")]
        [StringLength(150)]
        public string XTicketRoute { get; set; }
        [Column("N_TicketCount")]
        public int? NTicketCount { get; set; }
        [Column("X_TicketNotes")]
        [StringLength(100)]
        public string XTicketNotes { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("D_ProEndDate", TypeName = "datetime")]
        public DateTime? DProEndDate { get; set; }
        [Column("X_ClearanceCode")]
        [StringLength(50)]
        public string XClearanceCode { get; set; }
        [Column("I_Employe_Sign", TypeName = "image")]
        public byte[] IEmployeSign { get; set; }
        [Column("N_SalaryGrade")]
        public int? NSalaryGrade { get; set; }
        [Column("X_DrivingLicense")]
        [StringLength(100)]
        public string XDrivingLicense { get; set; }
        [Column("N_TravelClass")]
        public int? NTravelClass { get; set; }
        [Column("X_Religion")]
        [StringLength(100)]
        public string XReligion { get; set; }
        [Column("N_BloodGroupID")]
        public int? NBloodGroupId { get; set; }
        [Column("X_Heir")]
        [StringLength(200)]
        public string XHeir { get; set; }
        [Column("N_LoanAmountLimit", TypeName = "money")]
        public decimal? NLoanAmountLimit { get; set; }
        [Column("N_LoanCountLimit")]
        public int? NLoanCountLimit { get; set; }
        [Column("N_TicketTypeID")]
        public int? NTicketTypeId { get; set; }
        [Column("N_InsClassID")]
        public int? NInsClassId { get; set; }
        [Column("N_CostcenterId")]
        public int? NCostcenterId { get; set; }
        [Column("N_LoanEligible")]
        public int? NLoanEligible { get; set; }
        [Column("N_RefRecID")]
        public int? NRefRecId { get; set; }
        [Column("N_InsurancePayCode")]
        public int? NInsurancePayCode { get; set; }
        [Column("B_EnableDriver")]
        public bool? BEnableDriver { get; set; }
        [Column("N_ResidenceStatusID")]
        public int? NResidenceStatusId { get; set; }
        [Column("X_PlaceOfBirth")]
        [StringLength(200)]
        public string XPlaceOfBirth { get; set; }

        [ForeignKey("NDepartmentId,NFnYearId")]
        [InverseProperty(nameof(PayDepartment.PayEmployee))]
        public virtual PayDepartment N { get; set; }
        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.PayEmployee))]
        public virtual AccCompany NCompany { get; set; }
        [ForeignKey(nameof(NPositionId))]
        [InverseProperty(nameof(PayPosition.PayEmployee))]
        public virtual PayPosition NPosition { get; set; }
    }
}
