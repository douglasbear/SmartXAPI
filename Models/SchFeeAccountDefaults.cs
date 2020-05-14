using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeAccountDefaults")]
    public partial class SchFeeAccountDefaults
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_AccDefaultID")]
        public int NAccDefaultId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_FeeIncomeDefAccountID")]
        public int? NFeeIncomeDefAccountId { get; set; }
        [Column("N_FeeProposedIncomeDefAccountID")]
        public int? NFeeProposedIncomeDefAccountId { get; set; }
    }
}
