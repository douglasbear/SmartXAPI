using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ReportLedgerCategory")]
    public partial class AccReportLedgerCategory
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ReportCategoryID")]
        public int NReportCategoryId { get; set; }
        [Column("X_ReportCategoryDesc")]
        [StringLength(100)]
        public string XReportCategoryDesc { get; set; }
    }
}
