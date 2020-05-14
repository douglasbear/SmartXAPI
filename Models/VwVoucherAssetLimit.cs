using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVoucherAssetLimit
    {
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        [Column("N_Segment_5")]
        public int? NSegment5 { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
    }
}
