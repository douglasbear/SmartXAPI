using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_EmployeePayHistory")]
    public partial class PayEmployeePayHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PayHistoryID")]
        public int NPayHistoryId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_EffectiveDate", TypeName = "datetime")]
        public DateTime? DEffectiveDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_AmountOld", TypeName = "money")]
        public decimal? NAmountOld { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Percentage")]
        public double? NPercentage { get; set; }
        [Column("N_HistoryID")]
        public int? NHistoryId { get; set; }
    }
}
