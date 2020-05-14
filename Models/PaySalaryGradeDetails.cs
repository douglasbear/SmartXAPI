using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_SalaryGradeDetails")]
    public partial class PaySalaryGradeDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_GradeID")]
        public int NGradeId { get; set; }
        [Key]
        [Column("N_GradeDetailsID")]
        public int NGradeDetailsId { get; set; }
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
        [Column("N_ReferenceID")]
        public int? NReferenceId { get; set; }
        [Column("X_TicketType")]
        [StringLength(100)]
        public string XTicketType { get; set; }
        [Column("N_TicketAmount", TypeName = "money")]
        public decimal? NTicketAmount { get; set; }
        [Column("X_TicketNotes")]
        [StringLength(100)]
        public string XTicketNotes { get; set; }
    }
}
