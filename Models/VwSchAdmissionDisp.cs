using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmissionDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("Admission No")]
        [StringLength(25)]
        public string AdmissionNo { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [StringLength(50)]
        public string Class { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("Father Name")]
        [StringLength(200)]
        public string FatherName { get; set; }
        [Required]
        [Column("Mobile No")]
        [StringLength(50)]
        public string MobileNo { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Required]
        [Column("NationalID")]
        [StringLength(50)]
        public string NationalId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Required]
        [Column("X_GaurdianName")]
        [StringLength(250)]
        public string XGaurdianName { get; set; }
        [Required]
        [Column("X_GaurdianName_Ar")]
        [StringLength(250)]
        public string XGaurdianNameAr { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
    }
}
