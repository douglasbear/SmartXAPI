using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvVoucherDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("Voucher No")]
        [StringLength(50)]
        public string VoucherNo { get; set; }
        [Column("Voucher Date", TypeName = "datetime")]
        public DateTime? VoucherDate { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Required]
        [StringLength(5)]
        public string Posted { get; set; }
    }
}
