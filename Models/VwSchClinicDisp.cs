using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchClinicDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ClinicManagerID")]
        public int NClinicManagerId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("N_BloodGroupID")]
        public int? NBloodGroupId { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("X_ClinicCode")]
        [StringLength(50)]
        public string XClinicCode { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
