using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayTimeSheetEditToExcel
    {
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? WorkDate { get; set; }
        [Column("EmployeeID")]
        public int? EmployeeId { get; set; }
        public TimeSpan? InTime { get; set; }
        public TimeSpan? OutTime { get; set; }
        public TimeSpan? InTime2 { get; set; }
        public TimeSpan? OutTime2 { get; set; }
        [Column(TypeName = "money")]
        public decimal? WorkTime { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
