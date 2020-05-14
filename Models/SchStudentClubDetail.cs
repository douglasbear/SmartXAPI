using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentClubDetail")]
    public partial class SchStudentClubDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Key]
        [Column("N_ClubDetailID")]
        public int NClubDetailId { get; set; }
        [Column("N_ClubID")]
        public int NClubId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
