using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DeliveryNoteID")]
        public int? NDeliveryNoteId { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [StringLength(1000)]
        public string SalesAmountInWords { get; set; }
        [StringLength(1000)]
        public string TaxAmountInWords { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
    }
}
