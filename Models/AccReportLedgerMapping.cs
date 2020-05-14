using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ReportLedgerMapping")]
    public partial class AccReportLedgerMapping
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_LedgerMappingID")]
        public int NLedgerMappingId { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_ReportGroupID")]
        public int? NReportGroupId { get; set; }
        [Column("N_ReportCategoryID")]
        public int? NReportCategoryId { get; set; }
        [Column("N_OrderID")]
        public int? NOrderId { get; set; }
    }
}
