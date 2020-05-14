using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGenEntryDashboard
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_VoucherID")]
        public int NVoucherId { get; set; }
        [Required]
        [Column("X_ID")]
        [StringLength(20)]
        public string XId { get; set; }
        [Column("D_VoucherDate", TypeName = "datetime")]
        public DateTime? DVoucherDate { get; set; }
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("Party_ID")]
        public int PartyId { get; set; }
        [Column("Party_Name")]
        [StringLength(100)]
        public string PartyName { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("X_PayMethod")]
        [StringLength(50)]
        public string XPayMethod { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("B_IsDraft")]
        public bool? BIsDraft { get; set; }
        [Column("X_VoucherRemarks")]
        [StringLength(50)]
        public string XVoucherRemarks { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
    }
}
