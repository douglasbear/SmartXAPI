using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaFollowUpDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RegistrationID")]
        public int NRegistrationId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Column("N_ConsultantID")]
        public int NConsultantId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("D_FollowUpDate", TypeName = "datetime")]
        public DateTime? DFollowUpDate { get; set; }
        [Column("T_FollowUpTime", TypeName = "time(5)")]
        public TimeSpan? TFollowUpTime { get; set; }
        [Column("T_FollowUpToTime", TypeName = "time(5)")]
        public TimeSpan? TFollowUpToTime { get; set; }
        [Column("X_ConsultantCode")]
        [StringLength(25)]
        public string XConsultantCode { get; set; }
        [Column("X_ConsultantName")]
        [StringLength(60)]
        public string XConsultantName { get; set; }
        [Column("N_FollowUpID")]
        public int NFollowUpId { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
    }
}
