using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEndOfService
    {
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ServiceEndID")]
        public int NServiceEndId { get; set; }
        [Column("N_EndTypeID")]
        public int? NEndTypeId { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("X_ServiceEndCode")]
        [StringLength(50)]
        public string XServiceEndCode { get; set; }
        [Column("N_EOSDetailID")]
        public int NEosdetailId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_PayRate", TypeName = "money")]
        public decimal? NPayRate { get; set; }
        [Column("N_PayrollStatus")]
        public int? NPayrollStatus { get; set; }
        [Column("X_EmpCode")]
        [StringLength(50)]
        public string XEmpCode { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("N_PayMethod")]
        public int? NPayMethod { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_PayType")]
        public int? NPayType { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
    }
}
