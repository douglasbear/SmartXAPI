using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaParalegalDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ParalegalID")]
        public int NParalegalId { get; set; }
        [Column("X_ParalegalCode")]
        [StringLength(25)]
        public string XParalegalCode { get; set; }
        [Column("X_ParalegalName")]
        [StringLength(60)]
        public string XParalegalName { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("X_BranchCode")]
        [StringLength(50)]
        public string XBranchCode { get; set; }
        [Column("X_Email")]
        [StringLength(20)]
        public string XEmail { get; set; }
    }
}
