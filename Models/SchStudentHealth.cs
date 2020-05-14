using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentHealth")]
    public partial class SchStudentHealth
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_HealthID")]
        public int NHealthId { get; set; }
        [Column("N_ClinicID")]
        public int? NClinicId { get; set; }
        [Column("N_HealthInfoID")]
        public int? NHealthInfoId { get; set; }
        [Column("X_HealthRemarks")]
        [StringLength(50)]
        public string XHealthRemarks { get; set; }
        [Column("D_Healthdate", TypeName = "datetime")]
        public DateTime? DHealthdate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
