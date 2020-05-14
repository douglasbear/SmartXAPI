using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rst_TenentInvoice")]
    public partial class RstTenentInvoice
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_BatchID")]
        public int NBatchId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ProcessDate", TypeName = "smalldatetime")]
        public DateTime? DProcessDate { get; set; }
        [Column("N_Month")]
        public int? NMonth { get; set; }
        [Column("N_Year")]
        public int? NYear { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
