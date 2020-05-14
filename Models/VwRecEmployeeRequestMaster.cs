using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRecEmployeeRequestMaster
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("X_RequestCode")]
        [StringLength(500)]
        public string XRequestCode { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_RequestDate")]
        [StringLength(100)]
        public string DRequestDate { get; set; }
    }
}
