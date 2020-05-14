using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayCsvDetails
    {
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_BasicSalary", TypeName = "money")]
        public decimal? NBasicSalary { get; set; }
        [Column("N_HA", TypeName = "money")]
        public decimal? NHa { get; set; }
        [Column("N_OtherEarnings", TypeName = "money")]
        public decimal? NOtherEarnings { get; set; }
        [Column("N_OtherDeductions", TypeName = "money")]
        public decimal? NOtherDeductions { get; set; }
    }
}
