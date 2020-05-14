using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPorderItemDashboard
    {
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Required]
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_POrderDate")]
        [StringLength(8000)]
        public string DPorderDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_PODeliveryDate")]
        [StringLength(11)]
        public string DPodeliveryDate { get; set; }
        [Required]
        [Column("D_ExpDelDate")]
        [StringLength(8000)]
        public string DExpDelDate { get; set; }
        [Required]
        [Column("X_PurchaseOrderNo")]
        [StringLength(50)]
        public string XPurchaseOrderNo { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty", TypeName = "money")]
        public decimal? NQty { get; set; }
        [Column("N_PPrice", TypeName = "money")]
        public decimal? NPprice { get; set; }
        [Required]
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Required]
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Required]
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Required]
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Required]
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Required]
        [Column("D_SODeliveryDate")]
        [StringLength(8000)]
        public string DSodeliveryDate { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
    }
}
