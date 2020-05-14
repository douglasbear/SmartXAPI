using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssDepreciationUcsearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_DepriciationNo")]
        [StringLength(50)]
        public string XDepriciationNo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_RunDate")]
        [StringLength(8000)]
        public string DRunDate { get; set; }
    }
}
