using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentDashboard
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("D_AdmissionDate")]
        [StringLength(8000)]
        public string DAdmissionDate { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Required]
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Required]
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
        [Required]
        [Column("X_ContactPersonName1")]
        [StringLength(50)]
        public string XContactPersonName1 { get; set; }
        [Required]
        [Column("Student_MobileNo")]
        [StringLength(25)]
        public string StudentMobileNo { get; set; }
        [Required]
        [Column("X_PMobileNo")]
        [StringLength(25)]
        public string XPmobileNo { get; set; }
        [Required]
        [Column("Student_FeesAmount")]
        [StringLength(30)]
        public string StudentFeesAmount { get; set; }
        [Required]
        [Column("N_DiscountAmt")]
        [StringLength(30)]
        public string NDiscountAmt { get; set; }
        [Required]
        [Column("N_BalanceAmount")]
        [StringLength(30)]
        public string NBalanceAmount { get; set; }
        [Required]
        [Column("N_PaidAmount")]
        [StringLength(30)]
        public string NPaidAmount { get; set; }
        [Required]
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Required]
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column(TypeName = "money")]
        public decimal? FeeAmt { get; set; }
        [Column(TypeName = "money")]
        public decimal? DiscAmt { get; set; }
        [Column(TypeName = "money")]
        public decimal BalanceAmt { get; set; }
        [Column("Parent_MobileNo")]
        [StringLength(50)]
        public string ParentMobileNo { get; set; }
        [Column("X_PPhoneNo")]
        [StringLength(50)]
        public string XPphoneNo { get; set; }
        [Column("X_GuardianName")]
        [StringLength(250)]
        public string XGuardianName { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
