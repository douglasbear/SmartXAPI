using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ReportType")]
    public partial class AccReportType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ReportTypeID")]
        public int NReportTypeId { get; set; }
        [Column("X_ReportTypeDesc")]
        [StringLength(50)]
        public string XReportTypeDesc { get; set; }
    }
}
