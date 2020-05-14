using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeePaycodes
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Value", TypeName = "money")]
        public decimal? NValue { get; set; }
        [Column("N_Percentage")]
        public int NPercentage { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayType")]
        public int? NPayType { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_ConfigLevel")]
        public int? NConfigLevel { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("D_HireDate", TypeName = "datetime")]
        public DateTime? DHireDate { get; set; }
        [Column("D_EffectiveDate", TypeName = "datetime")]
        public DateTime? DEffectiveDate { get; set; }
        [Column("N_GOSIPayID")]
        public int? NGosipayId { get; set; }
        [Column("N_GOSIStart", TypeName = "datetime")]
        public DateTime? NGosistart { get; set; }
        [Column("N_Empcontribution", TypeName = "money")]
        public decimal? NEmpcontribution { get; set; }
        [Column("N_Compcontribution", TypeName = "money")]
        public decimal? NCompcontribution { get; set; }
        [Column("N_Frequency")]
        public int? NFrequency { get; set; }
        [Column("D_StatusDate", TypeName = "date")]
        public DateTime? DStatusDate { get; set; }
        [Column("N_SalaryPeriodAdjustment")]
        public int NSalaryPeriodAdjustment { get; set; }
        [Column("B_IssaveDraft")]
        public bool BIssaveDraft { get; set; }
    }
}
