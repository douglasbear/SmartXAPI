using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwReportLedgerMapping
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_LedgerMappingID")]
        public int NLedgerMappingId { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_ReportGroupID")]
        public int? NReportGroupId { get; set; }
        [Column("N_CategoryID")]
        public int NCategoryId { get; set; }
        [Column("N_OrderID")]
        public int? NOrderId { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
        [Column("X_CategoryName")]
        [StringLength(100)]
        public string XCategoryName { get; set; }
        [Column("X_ReportGroupDesc")]
        [StringLength(50)]
        public string XReportGroupDesc { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
