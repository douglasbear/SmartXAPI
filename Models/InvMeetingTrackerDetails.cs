﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_MeetingTrackerDetails")]
    public partial class InvMeetingTrackerDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_TrackID")]
        public int NTrackId { get; set; }
        [Key]
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
        [Column("X_NextAction")]
        [StringLength(5000)]
        public string XNextAction { get; set; }
    }
}
