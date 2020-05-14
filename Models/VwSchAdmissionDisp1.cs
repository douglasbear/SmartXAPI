using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmissionDisp1
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("Admission No")]
        [StringLength(30)]
        public string AdmissionNo { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(50)]
        public string Class { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("Father Name")]
        [StringLength(200)]
        public string FatherName { get; set; }
        [Column("Mobile No")]
        [StringLength(50)]
        public string MobileNo { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
    }
}
