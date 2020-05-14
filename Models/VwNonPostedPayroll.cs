using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwNonPostedPayroll
    {
        public long? Srl { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("Voucher No")]
        [StringLength(50)]
        public string VoucherNo { get; set; }
        [Column("Voucher Date", TypeName = "datetime")]
        public DateTime? VoucherDate { get; set; }
        [StringLength(500)]
        public string Remarks { get; set; }
        [Column("Reference No")]
        [StringLength(50)]
        public string ReferenceNo { get; set; }
        [Column(TypeName = "money")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "money")]
        public decimal? Credit { get; set; }
        public bool? Select { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_EntryFrom")]
        [StringLength(100)]
        public string XEntryFrom { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
    }
}
