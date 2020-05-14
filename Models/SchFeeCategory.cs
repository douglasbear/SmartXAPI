using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeCategory")]
    public partial class SchFeeCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_FeeCategoryID")]
        public int NFeeCategoryId { get; set; }
        [Column("X_FeeCategory")]
        [StringLength(50)]
        public string XFeeCategory { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_FeeIncomeDefAccountID")]
        public int? NFeeIncomeDefAccountId { get; set; }
        [Column("N_FeeProposedIncomeDefAccountID")]
        public int? NFeeProposedIncomeDefAccountId { get; set; }
        [Column("X_FeeCategoryCode")]
        [StringLength(50)]
        public string XFeeCategoryCode { get; set; }
        [Column("X_FeeCategory_Ar")]
        [StringLength(250)]
        public string XFeeCategoryAr { get; set; }
        [Column("N_Sort")]
        public int? NSort { get; set; }
        [Column("B_IsActive")]
        public bool? BIsActive { get; set; }
    }
}
