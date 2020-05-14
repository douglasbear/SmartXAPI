using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("prj_EmployeeWorkLog")]
    public partial class PrjEmployeeWorkLog
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_WorkLogID")]
        public int NWorkLogId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_MonthYear")]
        public int? NMonthYear { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_TimeSlotID")]
        public int NTimeSlotId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_MonthYear")]
        [StringLength(50)]
        public string XMonthYear { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
