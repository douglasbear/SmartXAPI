using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPurchaseOrderInvoiceNoSearch
    {
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("Order No")]
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("Order Date")]
        [StringLength(8000)]
        public string OrderDate { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
    }
}
