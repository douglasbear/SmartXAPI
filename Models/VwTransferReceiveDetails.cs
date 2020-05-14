using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTransferReceiveDetails
    {
        [StringLength(50)]
        public string TransferNo { get; set; }
        [Column("D_TransferDate")]
        [StringLength(8000)]
        public string DTransferDate { get; set; }
        [StringLength(50)]
        public string ReceiveNo { get; set; }
        [Column("D_ReceivableDate")]
        [StringLength(8000)]
        public string DReceivableDate { get; set; }
        public string TransLocation { get; set; }
        public string ReceiveLocation { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(10)]
        public string XStatus { get; set; }
    }
}
