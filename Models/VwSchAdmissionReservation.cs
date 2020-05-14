using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmissionReservation
    {
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_COB")]
        [StringLength(100)]
        public string XCob { get; set; }
        [Column("X_LastName")]
        [StringLength(200)]
        public string XLastName { get; set; }
        [Column("X_Initial")]
        [StringLength(200)]
        public string XInitial { get; set; }
        [Column("X_GivenName")]
        [StringLength(200)]
        public string XGivenName { get; set; }
        [Column("D_LeftDate", TypeName = "datetime")]
        public DateTime? DLeftDate { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("X_StudentMobile")]
        [StringLength(50)]
        public string XStudentMobile { get; set; }
        [Column("X_MiddleName")]
        [StringLength(200)]
        public string XMiddleName { get; set; }
        [Column("N_RegID")]
        public int NRegId { get; set; }
        [Required]
        [Column("X_RegNo")]
        [StringLength(25)]
        public string XRegNo { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("B_IsNewStudent")]
        public bool? BIsNewStudent { get; set; }
        [Column("N_ClassDivisionID")]
        public int? NClassDivisionId { get; set; }
        [Column("N_SessionID")]
        public int? NSessionId { get; set; }
        [Column("N_CFID")]
        public int? NCfid { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
    }
}
