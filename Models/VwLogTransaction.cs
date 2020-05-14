using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwLogTransaction
    {
        [StringLength(100)]
        public string Action { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("Entry From")]
        [StringLength(50)]
        public string EntryFrom { get; set; }
        [Column("Voucher No")]
        [StringLength(50)]
        public string VoucherNo { get; set; }
        [StringLength(50)]
        public string User { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("System Name")]
        [StringLength(100)]
        public string SystemName { get; set; }
        [Column("D_ActionDate", TypeName = "datetime")]
        public DateTime? DActionDate { get; set; }
        [StringLength(1000)]
        public string Remarks { get; set; }
    }
}
