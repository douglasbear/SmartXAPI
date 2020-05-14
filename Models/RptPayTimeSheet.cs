using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rpt_Pay_TimeSheet")]
    public partial class RptPayTimeSheet
    {
        [Column("N_VacTypeID")]
        public int NVacTypeId { get; set; }
        [Column("X_VacType")]
        [StringLength(100)]
        public string XVacType { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("D_In")]
        public TimeSpan? DIn { get; set; }
        [Column("D_Out")]
        public TimeSpan? DOut { get; set; }
        [Column("N_TotalHours")]
        public double? NTotalHours { get; set; }
        [Column("N_OT")]
        public double? NOt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_FnYear")]
        public int? NFnYear { get; set; }
    }
}
