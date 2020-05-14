using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssAssetCategoryDisp
    {
        [StringLength(30)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [StringLength(100)]
        public string Account { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
