using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchReservationDetailsExisting
    {
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_Inactive")]
        public int? NInactive { get; set; }
    }
}
