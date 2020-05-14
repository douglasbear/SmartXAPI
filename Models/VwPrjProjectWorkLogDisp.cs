using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjProjectWorkLogDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
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
        [Column("X_TimeSlot")]
        [StringLength(50)]
        public string XTimeSlot { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(50)]
        public string XProjectCode { get; set; }
        [Column("X_ProjectDescr")]
        [StringLength(100)]
        public string XProjectDescr { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
    }
}
