using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentEnrollmentDisp
    {
        [Column("N_RegID")]
        public int? NRegId { get; set; }
        [Column("X_RegNo")]
        [StringLength(25)]
        public string XRegNo { get; set; }
        [Column("D_AssessDate", TypeName = "datetime")]
        public DateTime? DAssessDate { get; set; }
        [Column("D_ReserveDate")]
        public string DReserveDate { get; set; }
        [Column("D_TransDate")]
        public string DTransDate { get; set; }
        [Required]
        [Column("N_Amount")]
        [StringLength(30)]
        public string NAmount { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_QatarID")]
        [StringLength(25)]
        public string NQatarId { get; set; }
        [Column("X_Address")]
        [StringLength(100)]
        public string XAddress { get; set; }
        [Column("X_Phone")]
        [StringLength(20)]
        public string XPhone { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("D_AdmissionDate")]
        public string DAdmissionDate { get; set; }
        [Column("X_Nationality")]
        [StringLength(50)]
        public string XNationality { get; set; }
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
        [Required]
        [Column("parentName")]
        [StringLength(200)]
        public string ParentName { get; set; }
        [Required]
        [StringLength(200)]
        public string Mothername { get; set; }
        [Column("X_PAddress")]
        [StringLength(150)]
        public string XPaddress { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Required]
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("Parent_MobileNo")]
        [StringLength(50)]
        public string ParentMobileNo { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Column("X_MiddleName")]
        [StringLength(50)]
        public string XMiddleName { get; set; }
        [Column("X_LastName")]
        [StringLength(50)]
        public string XLastName { get; set; }
        [Column("X_Initial")]
        [StringLength(50)]
        public string XInitial { get; set; }
        [Column("X_GivenName")]
        [StringLength(200)]
        public string XGivenName { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
