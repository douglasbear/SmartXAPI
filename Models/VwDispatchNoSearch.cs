using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDispatchNoSearch
    {
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Required]
        [Column("Dispatch No")]
        [StringLength(50)]
        public string DispatchNo { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DispatchID")]
        public int NDispatchId { get; set; }
        [Column("X_Location")]
        [StringLength(200)]
        public string XLocation { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Required]
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Required]
        [Column("Delivery Address")]
        [StringLength(500)]
        public string DeliveryAddress { get; set; }
        [Column("Delivery Date")]
        [StringLength(8000)]
        public string DeliveryDate { get; set; }
        [Column("Delivery Time")]
        [StringLength(8)]
        public string DeliveryTime { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("B_DispatchScheduled")]
        public bool BDispatchScheduled { get; set; }
        [Required]
        [Column("Dispatch Status")]
        [StringLength(10)]
        public string DispatchStatus { get; set; }
    }
}
