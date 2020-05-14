using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_MeetingTracker")]
    public partial class InvMeetingTracker
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("D_MeetingDate", TypeName = "datetime")]
        public DateTime? DMeetingDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Key]
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(200)]
        public string XReferenceNo { get; set; }
        [Column("X_Notes")]
        [StringLength(5000)]
        public string XNotes { get; set; }
    }
}
