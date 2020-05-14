using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_BOQ")]
    public partial class PrjBoq
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_BOQID")]
        public int NBoqid { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_TaxCategoryID")]
        public int? NTaxCategoryId { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_productAmt", TypeName = "money")]
        public decimal? NProductAmt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_FormType")]
        public int? NFormType { get; set; }
        [Column("N_TenderID")]
        public int? NTenderId { get; set; }
        [Column("N_ActualAmt", TypeName = "money")]
        public decimal? NActualAmt { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_ApprovalLevelId")]
        public int? NApprovalLevelId { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
    }
}
