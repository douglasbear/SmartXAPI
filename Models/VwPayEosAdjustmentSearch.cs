using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEosAdjustmentSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AdjustmentID")]
        public int NAdjustmentId { get; set; }
        public string Code { get; set; }
        [Column("Adjustment Date")]
        [StringLength(8000)]
        public string AdjustmentDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("Adjustment Amount")]
        [StringLength(11)]
        public string AdjustmentAmount { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("D_AdjustmentDate", TypeName = "datetime")]
        public DateTime? DAdjustmentDate { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayID")]
        public int NPayId { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
    }
}
