using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMeetingTrackerDetailsRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Column("N_MeetingTrackerDetailsID")]
        public int NMeetingTrackerDetailsId { get; set; }
        [Column("X_Activity")]
        [StringLength(200)]
        public string XActivity { get; set; }
        [Column("X_ActionToBeTaken")]
        [StringLength(200)]
        public string XActionToBeTaken { get; set; }
        [Column("X_ActionTaken")]
        [StringLength(200)]
        public string XActionTaken { get; set; }
        [Column("D_TargetDate", TypeName = "datetime")]
        public DateTime? DTargetDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(200)]
        public string XRemarks { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("D_MeetingDate", TypeName = "datetime")]
        public DateTime? DMeetingDate { get; set; }
        [Column("X_NextAction")]
        [StringLength(5000)]
        public string XNextAction { get; set; }
    }
}
