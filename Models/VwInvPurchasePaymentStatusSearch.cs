using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchasePaymentStatusSearch
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnyearID")]
        public int NFnyearId { get; set; }
        [Column("N_PaymentStatusID")]
        public int NPaymentStatusId { get; set; }
        [Column("X_StatusCode")]
        [StringLength(50)]
        public string XStatusCode { get; set; }
        [StringLength(8000)]
        public string RefDate { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Required]
        [Column("X_VoucherNo")]
        [StringLength(50)]
        public string XVoucherNo { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
    }
}
