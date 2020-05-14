using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaySetupForRevision
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PaySetupID")]
        public int NPaySetupId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("X_PayType")]
        [StringLength(100)]
        public string XPayType { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("N_PayRate", TypeName = "money")]
        public decimal? NPayRate { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("X_PayGroup")]
        [StringLength(50)]
        public string XPayGroup { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("X_Method")]
        [StringLength(100)]
        public string XMethod { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_ParentPayCodeID")]
        public int? NParentPayCodeId { get; set; }
        [Column("X_ParentPayCode")]
        [StringLength(50)]
        public string XParentPayCode { get; set; }
        [Column("B_EndDate")]
        public bool? BEndDate { get; set; }
        [Column("B_StartDate")]
        public bool? BStartDate { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_PayHistoryID")]
        public int NPayHistoryId { get; set; }
        [Column("D_EffectiveDate", TypeName = "datetime")]
        public DateTime? DEffectiveDate { get; set; }
    }
}
