using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvServiceBookingDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate")]
        [StringLength(8000)]
        public string DSalesDate { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_IMEITo")]
        [StringLength(50)]
        public string NImeito { get; set; }
        [Required]
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("X_Type")]
        [StringLength(20)]
        public string XType { get; set; }
    }
}
