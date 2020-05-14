using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeePayHistoryRpt
    {
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
        [Column("N_HistoryID")]
        public int? NHistoryId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [StringLength(8000)]
        public string EffectiveDate { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
    }
}
