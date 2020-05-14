using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentAttendence_Detail")]
    public partial class SchStudentAttendenceDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EntryID")]
        public int NEntryId { get; set; }
        [Key]
        [Column("N_EntryDetailID")]
        public int NEntryDetailId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("B_Present")]
        public bool BPresent { get; set; }
        [Column("X_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
