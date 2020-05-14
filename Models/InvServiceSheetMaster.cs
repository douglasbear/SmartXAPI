using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ServiceSheetMaster")]
    public partial class InvServiceSheetMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ServiceSheetID")]
        public int NServiceSheetId { get; set; }
        [Required]
        [Column("X_ServiceSheetCode")]
        [StringLength(100)]
        public string XServiceSheetCode { get; set; }
        [Column("D_Invoicedate", TypeName = "datetime")]
        public DateTime DInvoicedate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PayRunID")]
        public int? NPayRunId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("X_Type")]
        [StringLength(100)]
        public string XType { get; set; }
    }
}
