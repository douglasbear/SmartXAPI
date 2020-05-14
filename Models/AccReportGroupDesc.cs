using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ReportGroupDesc")]
    public partial class AccReportGroupDesc
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ReportGroupID")]
        public int NReportGroupId { get; set; }
        [Column("X_ReportGroupDesc")]
        [StringLength(50)]
        public string XReportGroupDesc { get; set; }
        [Column("N_ReportTypeID")]
        public int? NReportTypeId { get; set; }
    }
}
