using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeCount
    {
        [Column("X_FnYearDescr")]
        [StringLength(20)]
        public string XFnYearDescr { get; set; }
        [Column("X_Month")]
        [StringLength(10)]
        public string XMonth { get; set; }
        [Column("N_monthID")]
        public int? NMonthId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
    }
}
