using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrnReport
    {
        [Column("D_MRNDate", TypeName = "datetime")]
        public DateTime? DMrndate { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("X_POrderNo")]
        [StringLength(54)]
        public string XPorderNo { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_QtyToStock")]
        public int? NQtyToStock { get; set; }
        [Column("N_ReturnQty")]
        public int? NReturnQty { get; set; }
        [Column("X_Reason")]
        [StringLength(50)]
        public string XReason { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
    }
}
