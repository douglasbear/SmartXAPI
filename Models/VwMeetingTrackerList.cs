using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMeetingTrackerList
    {
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(200)]
        public string XReferenceNo { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [StringLength(30)]
        public string ActivityCount { get; set; }
        [StringLength(8000)]
        public string MeetingDate { get; set; }
        [Column("D_MeetingDate", TypeName = "datetime")]
        public DateTime? DMeetingDate { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
    }
}
