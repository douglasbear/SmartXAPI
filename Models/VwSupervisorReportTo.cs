using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSupervisorReportTo
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SupervisorID")]
        public int NSupervisorId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Required]
        [Column("Employee Code")]
        [StringLength(50)]
        public string EmployeeCode { get; set; }
        [Required]
        [Column("Employee Name")]
        [StringLength(100)]
        public string EmployeeName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
