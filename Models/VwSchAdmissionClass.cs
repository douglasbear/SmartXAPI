using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmissionClass
    {
        [Column("N_AdmissionClassID")]
        public int NAdmissionClassId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("B_Active")]
        public bool BActive { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Required]
        [Column("X_AcYear")]
        [StringLength(50)]
        public string XAcYear { get; set; }
        [Column("D_YearFrom", TypeName = "datetime")]
        public DateTime? DYearFrom { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_NameLocale")]
        [StringLength(250)]
        public string XNameLocale { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
    }
}
