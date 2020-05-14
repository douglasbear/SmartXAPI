using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_AcademicYear")]
    public partial class SchAcademicYear
    {
        public SchAcademicYear()
        {
            SchAdmission = new HashSet<SchAdmission>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Required]
        [Column("X_AcYear")]
        [StringLength(50)]
        public string XAcYear { get; set; }
        [Column("D_YearFrom", TypeName = "datetime")]
        public DateTime? DYearFrom { get; set; }
        [Column("D_YearTo", TypeName = "datetime")]
        public DateTime? DYearTo { get; set; }
        [Column("B_YearClosed")]
        public bool? BYearClosed { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [InverseProperty("NAcYear")]
        public virtual ICollection<SchAdmission> SchAdmission { get; set; }
    }
}
