using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStudentAttendanceDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EntryID")]
        public int NEntryId { get; set; }
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
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("N_ParentID")]
        public int? NParentId { get; set; }
    }
}
