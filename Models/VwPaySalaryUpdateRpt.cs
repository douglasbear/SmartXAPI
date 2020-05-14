using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaySalaryUpdateRpt
    {
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_AmountOld", TypeName = "money")]
        public decimal? NAmountOld { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CurrentDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? HireDate { get; set; }
        [Required]
        [StringLength(7)]
        public string Status { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        public int MonthNmbr { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}
