using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecAppointmentDisp
    {
        [Column("X_FileNo")]
        public int? XFileNo { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_RecruitmentCode")]
        [StringLength(400)]
        public string XRecruitmentCode { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(100)]
        public string XEmail { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
    }
}
