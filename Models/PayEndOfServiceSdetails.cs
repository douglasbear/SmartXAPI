using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("pay_EndOfServiceSDetails")]
    public partial class PayEndOfServiceSdetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_EOSDetailID")]
        public int NEosdetailId { get; set; }
        [Column("N_ServiceEndID")]
        public int? NServiceEndId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_PayRate", TypeName = "money")]
        public decimal? NPayRate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_PayrollStatus")]
        public int? NPayrollStatus { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
    }
}
