using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_MonthlyAddOrDedDetails")]
    public partial class PayMonthlyAddOrDedDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TransDetailsID")]
        public int NTransDetailsId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("N_HrsOrDays")]
        public double? NHrsOrDays { get; set; }
        [Column("N_ApplytoAmount", TypeName = "money")]
        public decimal? NApplytoAmount { get; set; }
        [Column("B_Posted")]
        public bool? BPosted { get; set; }
        [Column("D_PostedDate", TypeName = "datetime")]
        public DateTime? DPostedDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_TimeSheetEntry")]
        public bool? BTimeSheetEntry { get; set; }
        [Column("X_Remarks")]
        [StringLength(250)]
        public string XRemarks { get; set; }
        [Column("N_refID")]
        public int? NRefId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_TotalDays")]
        public double? NTotalDays { get; set; }
    }
}
