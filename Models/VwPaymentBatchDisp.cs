using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaymentBatchDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TransDetailsID")]
        public int NTransDetailsId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("N_PayFactor")]
        public double? NPayFactor { get; set; }
        [Column("B_Posted")]
        public bool? BPosted { get; set; }
        [Column("D_PostedDate", TypeName = "datetime")]
        public DateTime? DPostedDate { get; set; }
        [Column("N_SalaryPayMethod")]
        public int? NSalaryPayMethod { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("X_Remarks")]
        public string XRemarks { get; set; }
        [Column("D_DateTo", TypeName = "date")]
        public DateTime? DDateTo { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
    }
}
