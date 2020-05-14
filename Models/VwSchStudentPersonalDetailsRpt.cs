using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentPersonalDetailsRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmittedClassID")]
        public int NAdmittedClassId { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("D_AdmissionDate", TypeName = "datetime")]
        public DateTime? DAdmissionDate { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("X_BloodGroup")]
        [StringLength(25)]
        public string XBloodGroup { get; set; }
        [Column("X_ContactPersonName1")]
        [StringLength(50)]
        public string XContactPersonName1 { get; set; }
        [Column("X_ContactPersonNo1")]
        [StringLength(25)]
        public string XContactPersonNo1 { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        public int Expr1 { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_PlaceofBirth")]
        [StringLength(500)]
        public string XPlaceofBirth { get; set; }
        [Column("X_StudentMobile")]
        [StringLength(50)]
        public string XStudentMobile { get; set; }
        [Column("N_ParentID")]
        public int? NParentId { get; set; }
        [Column("X_PFatherName")]
        [StringLength(200)]
        public string XPfatherName { get; set; }
        [Column("X_PMotherName")]
        [StringLength(200)]
        public string XPmotherName { get; set; }
        [Column("X_PAddress")]
        [StringLength(150)]
        public string XPaddress { get; set; }
        [Column("X_PCity")]
        [StringLength(50)]
        public string XPcity { get; set; }
        [Column("X_JobF")]
        [StringLength(100)]
        public string XJobF { get; set; }
        [Column("X_CompanyF")]
        [StringLength(100)]
        public string XCompanyF { get; set; }
    }
}
