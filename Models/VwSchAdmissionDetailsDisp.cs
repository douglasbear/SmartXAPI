using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmissionDetailsDisp
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
        [Column("X_NameLocale")]
        [StringLength(250)]
        public string XNameLocale { get; set; }
        [StringLength(50)]
        public string Class { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("Father Name")]
        [StringLength(200)]
        public string FatherName { get; set; }
        [Column("Gaurdian_Name")]
        [StringLength(250)]
        public string GaurdianName { get; set; }
        [Column("Gaurdian_NameLocale")]
        [StringLength(250)]
        public string GaurdianNameLocale { get; set; }
        [Column("Mobile No")]
        [StringLength(50)]
        public string MobileNo { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_BookCode")]
        [StringLength(20)]
        public string XBookCode { get; set; }
    }
}
