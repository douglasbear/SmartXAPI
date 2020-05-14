using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesInvoiceStatusRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [StringLength(50)]
        public string DelNo { get; set; }
        [StringLength(50)]
        public string InvNo { get; set; }
        [StringLength(50)]
        public string RetNo { get; set; }
        [Column("D_DeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DDeliveryDate { get; set; }
        [Column("N_DelQty")]
        public double? NDelQty { get; set; }
        [Column("N_SalesQty")]
        public double? NSalesQty { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
    }
}
