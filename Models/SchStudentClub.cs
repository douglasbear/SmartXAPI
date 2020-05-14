using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentClub")]
    public partial class SchStudentClub
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PkeyID")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(20)]
        public string XPkeyCode { get; set; }
        [Column("X_Schedule")]
        [StringLength(250)]
        public string XSchedule { get; set; }
        [Column("X_ClubName")]
        [StringLength(250)]
        public string XClubName { get; set; }
        [Column("N_ModeratorID")]
        public int NModeratorId { get; set; }
        [Column("D_Day")]
        [StringLength(250)]
        public string DDay { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ClubID")]
        public int? NClubId { get; set; }
    }
}
