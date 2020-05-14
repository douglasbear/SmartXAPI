using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMeetingTracker
    {
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_MeetingDate", TypeName = "datetime")]
        public DateTime? DMeetingDate { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(200)]
        public string XReferenceNo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Column("X_Notes")]
        [StringLength(5000)]
        public string XNotes { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
    }
}
