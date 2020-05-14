using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PaySetup")]
    public partial class PayPaySetup
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_PaySetupID")]
        public int NPaySetupId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("N_PayRate", TypeName = "money")]
        public decimal? NPayRate { get; set; }
        [Column("B_StartDate")]
        public bool? BStartDate { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("B_EndDate")]
        public bool? BEndDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("X_Method")]
        [StringLength(100)]
        public string XMethod { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
