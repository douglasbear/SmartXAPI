using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmpGrpWorkhours
    {
        [Column("X_Day")]
        [StringLength(50)]
        public string XDay { get; set; }
        [Column("N_Workhours")]
        public double? NWorkhours { get; set; }
        [Column("N_MinWorkhours")]
        public double? NMinWorkhours { get; set; }
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(50)]
        public string XGroupName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("B_Addition")]
        public bool? BAddition { get; set; }
        [Column("B_Deduction")]
        public bool? BDeduction { get; set; }
        [Column("B_Compensation")]
        public bool? BCompensation { get; set; }
        [Column("N_Compansate_Minutes")]
        public int? NCompansateMinutes { get; set; }
        [Column("N_MonthlyWorkhour")]
        public int? NMonthlyWorkhour { get; set; }
        [Column("B_MonthlyHour")]
        public bool? BMonthlyHour { get; set; }
    }
}
