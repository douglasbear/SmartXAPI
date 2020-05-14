using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchStudentResidentialExpiry
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("D_DOB", TypeName = "datetime")]
        public DateTime? DDob { get; set; }
        [Column("N_ParentID")]
        public int? NParentId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        public int? Expr1 { get; set; }
        public int? Expr2 { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_GaurdianName")]
        [StringLength(250)]
        public string XGaurdianName { get; set; }
        public int? Expr3 { get; set; }
        [Column("X_PPhoneNo")]
        [StringLength(50)]
        public string XPphoneNo { get; set; }
        [Column("X_PMobileNo")]
        [StringLength(50)]
        public string XPmobileNo { get; set; }
        [Column("D_ExpiryDate", TypeName = "datetime")]
        public DateTime? DExpiryDate { get; set; }
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
    }
}
