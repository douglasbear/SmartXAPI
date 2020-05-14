using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_DAO_VacationDetail")]
    public partial class DaoVacationDetail
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Employee_id")]
        [StringLength(200)]
        public string EmployeeId { get; set; }
        [Column("Start_Date", TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column("End_Date", TypeName = "date")]
        public DateTime? EndDate { get; set; }
        [Column("Requested_Days")]
        public double? RequestedDays { get; set; }
    }
}
