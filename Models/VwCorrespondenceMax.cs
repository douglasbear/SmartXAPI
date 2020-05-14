using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCorrespondenceMax
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CorrespondenceId")]
        public int NCorrespondenceId { get; set; }
        [Column("Max_CurDetailsNo")]
        public int? MaxCurDetailsNo { get; set; }
    }
}
