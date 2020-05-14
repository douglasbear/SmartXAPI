using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvoiceAnalysisSummaryRpt
    {
        [Column("X_CompanyName")]
        [StringLength(250)]
        public string XCompanyName { get; set; }
        [Column("X_Country")]
        [StringLength(50)]
        public string XCountry { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("D_InvoiceDate", TypeName = "smalldatetime")]
        public DateTime? DInvoiceDate { get; set; }
        [Column("N_InvoiceId")]
        public int NInvoiceId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("Attachment_Count")]
        public int? AttachmentCount { get; set; }
    }
}
