using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRstTenentInvoiceDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BatchID")]
        public int NBatchId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ProcessDate")]
        [StringLength(8000)]
        public string DProcessDate { get; set; }
        [Column("X_Month")]
        [StringLength(30)]
        public string XMonth { get; set; }
        [Column("X_Year")]
        [StringLength(10)]
        public string XYear { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
