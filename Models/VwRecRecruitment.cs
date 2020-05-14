using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecRecruitment
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RecruitmentID")]
        public int NRecruitmentId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_Address")]
        [StringLength(400)]
        public string XAddress { get; set; }
        [Column("X_City")]
        [StringLength(100)]
        public string XCity { get; set; }
        [Column("X_EduMajor")]
        [StringLength(100)]
        public string XEduMajor { get; set; }
        [Column("N_ExperianceInYears")]
        public int NExperianceInYears { get; set; }
        [Column("X_ContactNo")]
        [StringLength(20)]
        public string XContactNo { get; set; }
        [Column("X_Email")]
        [StringLength(100)]
        public string XEmail { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime DDob { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Nationality")]
        [StringLength(100)]
        public string XNationality { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_NationalityID")]
        public int NNationalityId { get; set; }
        [Column("X_Religion")]
        [StringLength(100)]
        public string XReligion { get; set; }
        [Column("X_IDNo")]
        [StringLength(100)]
        public string XIdno { get; set; }
        [Column("X_MaritalStatus")]
        [StringLength(20)]
        public string XMaritalStatus { get; set; }
        [Column("X_EduBackground")]
        [StringLength(100)]
        public string XEduBackground { get; set; }
        [Column("N_PassoutYear")]
        public int NPassoutYear { get; set; }
        [Column("X_PostboxNo")]
        [StringLength(20)]
        public string XPostboxNo { get; set; }
        [Column("X_Telephone")]
        [StringLength(20)]
        public string XTelephone { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("X_Country")]
        [StringLength(300)]
        public string XCountry { get; set; }
        [Column("X_Vacancy")]
        [StringLength(100)]
        public string XVacancy { get; set; }
        [Column("X_LastCompany")]
        [StringLength(100)]
        public string XLastCompany { get; set; }
        [Column("X_LastPosition")]
        [StringLength(100)]
        public string XLastPosition { get; set; }
        [Required]
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_EmpID")]
        [StringLength(1000)]
        public string NEmpId { get; set; }
        [Column("X_IntName")]
        [StringLength(1000)]
        public string XIntName { get; set; }
        [Column("X_PassPortNo")]
        [StringLength(500)]
        public string XPassPortNo { get; set; }
        [Column("D_IssueDate", TypeName = "datetime")]
        public DateTime? DIssueDate { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("X_LicenceNo")]
        [StringLength(50)]
        public string XLicenceNo { get; set; }
        [Column("X_Sex")]
        [StringLength(200)]
        public string XSex { get; set; }
        [Column("N_RefEmpID")]
        public int? NRefEmpId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("I_Employe_Image", TypeName = "image")]
        public byte[] IEmployeImage { get; set; }
        [Column("I_Employe_Sign", TypeName = "image")]
        public byte[] IEmployeSign { get; set; }
        [Column("N_CurrentSalary", TypeName = "money")]
        public decimal NCurrentSalary { get; set; }
        [Column("N_ExpectedSalary", TypeName = "money")]
        public decimal NExpectedSalary { get; set; }
        [Column("N_RecTypeID")]
        public int NRecTypeId { get; set; }
        [Required]
        [Column("X_CurrrentLocation")]
        [StringLength(100)]
        public string XCurrrentLocation { get; set; }
        [Column("N_NoticePeriod")]
        public double NNoticePeriod { get; set; }
        [Required]
        [Column("X_ApprovalName")]
        [StringLength(50)]
        public string XApprovalName { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_VisaNo")]
        [StringLength(50)]
        public string XVisaNo { get; set; }
        [Column("X_VisaTitle")]
        [StringLength(50)]
        public string XVisaTitle { get; set; }
        [Required]
        [Column("X_FlightNo")]
        [StringLength(100)]
        public string XFlightNo { get; set; }
        [Required]
        [Column("X_Pickupman")]
        [StringLength(100)]
        public string XPickupman { get; set; }
        [Column("N_PickupmanID")]
        public int NPickupmanId { get; set; }
        [Column("D_ArrivedDate", TypeName = "datetime")]
        public DateTime? DArrivedDate { get; set; }
        [Column("X_FileName")]
        [StringLength(1000)]
        public string XFileName { get; set; }
        [Column("N_BloodGroupID")]
        public int? NBloodGroupId { get; set; }
    }
}
