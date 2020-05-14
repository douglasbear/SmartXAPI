using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMeetingTrackerDashboard
    {
        [Required]
        [StringLength(100)]
        public string MeetingCategory { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Required]
        public string MeetingDate { get; set; }
        [StringLength(50)]
        public string ReferenceNo { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Required]
        [StringLength(200)]
        public string ActionToBeTaken { get; set; }
        [Required]
        [StringLength(200)]
        public string ActionTaken { get; set; }
        [Required]
        [StringLength(100)]
        public string ActionOwner { get; set; }
        [Required]
        public string TargetDate { get; set; }
        [Required]
        [StringLength(200)]
        public string Remarks { get; set; }
        [Required]
        [StringLength(50)]
        public string MeetingStatus { get; set; }
        [Required]
        [StringLength(200)]
        public string Activity { get; set; }
        [Column("N_MeetingTrackerDetailsID")]
        public int NMeetingTrackerDetailsId { get; set; }
    }
}
