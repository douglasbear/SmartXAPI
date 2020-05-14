using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDeliveryDispatch1
    {
        [Required]
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Required]
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [Column("Customer Address")]
        [StringLength(250)]
        public string CustomerAddress { get; set; }
        [Required]
        [Column("Phone No")]
        [StringLength(20)]
        public string PhoneNo { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Required]
        [StringLength(8)]
        public string Status { get; set; }
        [Column("Schedule Date", TypeName = "datetime")]
        public DateTime? ScheduleDate { get; set; }
    }
}
