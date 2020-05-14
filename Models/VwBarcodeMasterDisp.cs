using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwBarcodeMasterDisp
    {
        [Required]
        [StringLength(8)]
        public string Type { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("D_TransDate")]
        [StringLength(15)]
        public string DTransDate { get; set; }
        [Column("X_TransactionNo")]
        [StringLength(50)]
        public string XTransactionNo { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
