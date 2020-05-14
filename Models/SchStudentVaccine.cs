using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentVaccine")]
    public partial class SchStudentVaccine
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VaccineID")]
        public int NVaccineId { get; set; }
        [Column("N_ClinicID")]
        public int? NClinicId { get; set; }
        [Column("N_VaccineInfoID")]
        public int? NVaccineInfoId { get; set; }
        [Column("N_VaccineYear")]
        public int NVaccineYear { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
