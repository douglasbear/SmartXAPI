using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVacationDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("Pay_Month")]
        [StringLength(30)]
        public string PayMonth { get; set; }
        [Column("N_VacDays")]
        public double? NVacDays { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayType")]
        public int? NPayType { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("B_DeductSalary")]
        public bool? BDeductSalary { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
    }
}
