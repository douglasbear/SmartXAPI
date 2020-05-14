using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMeetingTrackerHistory
    {
        [Required]
        [Column("X_Code")]
        [StringLength(200)]
        public string XCode { get; set; }
        [Column("N_MeetingTrackerDetailsID")]
        public int NMeetingTrackerDetailsId { get; set; }
        [Column("X_Activity")]
        [StringLength(200)]
        public string XActivity { get; set; }
        [Column("X_ActionTaken")]
        [StringLength(200)]
        public string XActionTaken { get; set; }
        [Column("X_ActionToBeTaken")]
        [StringLength(200)]
        public string XActionToBeTaken { get; set; }
        [Column("X_NextAction")]
        [StringLength(5000)]
        public string XNextAction { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [StringLength(8000)]
        public string MeetingDate { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_Notes")]
        [StringLength(5000)]
        public string XNotes { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("X_ActionOwner")]
        [StringLength(100)]
        public string XActionOwner { get; set; }
        [StringLength(8000)]
        public string TargetDate { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [StringLength(50)]
        public string StatusName { get; set; }
    }
}
