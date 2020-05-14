using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeAccountDefaults
    {
        [Column("N_AccDefaultID")]
        public int? NAccDefaultId { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("N_FeeIncomeDefAccountID")]
        public int? NFeeIncomeDefAccountId { get; set; }
        [Column("N_FeeProposedIncomeDefAccountID")]
        public int? NFeeProposedIncomeDefAccountId { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("X_FeeIncomeDefAccountCode")]
        [StringLength(50)]
        public string XFeeIncomeDefAccountCode { get; set; }
        [Column("X_FeeIncomeDefAccountName")]
        [StringLength(100)]
        public string XFeeIncomeDefAccountName { get; set; }
        [Column("X_FeeProposedIncomeDefAccountCode")]
        [StringLength(50)]
        public string XFeeProposedIncomeDefAccountCode { get; set; }
        [Column("X_FeeProposedIncomeDefAccountName")]
        [StringLength(100)]
        public string XFeeProposedIncomeDefAccountName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_FeeCategory")]
        [StringLength(50)]
        public string XFeeCategory { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
    }
}
