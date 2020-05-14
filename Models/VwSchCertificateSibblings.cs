using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchCertificateSibblings
    {
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_CompanyName")]
        [StringLength(250)]
        public string XCompanyName { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("D_YearFrom", TypeName = "datetime")]
        public DateTime? DYearFrom { get; set; }
        [Column("D_YearTo", TypeName = "datetime")]
        public DateTime? DYearTo { get; set; }
        [Column("X_NationalID")]
        [StringLength(50)]
        public string XNationalId { get; set; }
        [Column("N_ParentID")]
        public int NParentId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
    }
}
