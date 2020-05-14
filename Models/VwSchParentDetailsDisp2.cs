using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchParentDetailsDisp2
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(30)]
        public string Code { get; set; }
        [Column("Father Name")]
        [StringLength(200)]
        public string FatherName { get; set; }
        [Required]
        [Column("Mother Name")]
        [StringLength(200)]
        public string MotherName { get; set; }
        [Column("Family Name")]
        [StringLength(50)]
        public string FamilyName { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_GaurdianName")]
        [StringLength(250)]
        public string XGaurdianName { get; set; }
        [Column("X_GaurdianName_Ar")]
        [StringLength(250)]
        public string XGaurdianNameAr { get; set; }
        [Column("N_SiblingsCount")]
        public int NSiblingsCount { get; set; }
        [Column("X_ParentCode")]
        [StringLength(100)]
        public string XParentCode { get; set; }
        [Column("X_PNationalIDF")]
        [StringLength(50)]
        public string XPnationalIdf { get; set; }
    }
}
