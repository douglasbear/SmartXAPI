using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAccountUsers
    {
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
