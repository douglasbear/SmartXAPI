using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwResevationEnroledDetailRpt
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Required]
        [Column("X_RegNo")]
        [StringLength(25)]
        public string XRegNo { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Required]
        [StringLength(15)]
        public string Status1 { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_SessionID")]
        public int NSessionId { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
    }
}
