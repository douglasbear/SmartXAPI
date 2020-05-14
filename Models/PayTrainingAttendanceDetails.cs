using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_TrainingAttendanceDetails")]
    public partial class PayTrainingAttendanceDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AttendanceID")]
        public int? NAttendanceId { get; set; }
        [Key]
        [Column("N_AttendanceDetailsID")]
        public int NAttendanceDetailsId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("B_Present")]
        public bool? BPresent { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
    }
}
