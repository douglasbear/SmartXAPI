using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvRequestQuotationRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_RequestId")]
        public int? NRequestId { get; set; }
        [Column("X_RequestNo")]
        [StringLength(50)]
        public string XRequestNo { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_TotalAmount")]
        public double? NTotalAmount { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CurrencyID")]
        public int? NCurrencyId { get; set; }
        [Column("N_RequestDetailsID")]
        public int NRequestDetailsId { get; set; }
    }
}
