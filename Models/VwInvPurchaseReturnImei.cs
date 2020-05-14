using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseReturnImei
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CreditNoteId")]
        public int NCreditNoteId { get; set; }
        [Column("X_CreditNoteNo")]
        [StringLength(50)]
        public string XCreditNoteNo { get; set; }
        [Column("N_CreditNoteDetailsID")]
        public int NCreditNoteDetailsId { get; set; }
        [Column("N_PurchaseDetailsId")]
        public int? NPurchaseDetailsId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Expr2 { get; set; }
        public int? Expr3 { get; set; }
        public int? Expr4 { get; set; }
        [Column("X_IMEI")]
        [StringLength(50)]
        public string XImei { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
    }
}
