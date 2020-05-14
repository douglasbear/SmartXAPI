using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeePayHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PayHistoryID")]
        public int NPayHistoryId { get; set; }
        [Column("D_EffectiveDate", TypeName = "datetime")]
        public DateTime? DEffectiveDate { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_AmountOld", TypeName = "money")]
        public decimal? NAmountOld { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_Percentage")]
        public double? NPercentage { get; set; }
    }
}
