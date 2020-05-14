using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeeGroup")]
    public partial class PayEmployeeGroup
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(50)]
        public string XGroupName { get; set; }
        [Column("D_EntryDate", TypeName = "date")]
        public DateTime? DEntryDate { get; set; }
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
