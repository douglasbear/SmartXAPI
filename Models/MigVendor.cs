using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Mig_Vendor")]
    public partial class MigVendor
    {
        [Column("Vendor_code")]
        [StringLength(200)]
        public string VendorCode { get; set; }
        [Column("Vendor_Name")]
        [StringLength(200)]
        public string VendorName { get; set; }
        [StringLength(300)]
        public string Address { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(100)]
        public string State { get; set; }
        [StringLength(100)]
        public string Country { get; set; }
        [StringLength(100)]
        public string PinCode { get; set; }
        [StringLength(100)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Fax { get; set; }
        [StringLength(100)]
        public string Mobile { get; set; }
        [StringLength(100)]
        public string Pager { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(100)]
        public string ContactPer { get; set; }
        [StringLength(100)]
        public string Discount { get; set; }
        [StringLength(100)]
        public string Credit { get; set; }
        [Column("Customer_Name")]
        [StringLength(200)]
        public string CustomerName { get; set; }
        [Column("LineID")]
        [StringLength(100)]
        public string LineId { get; set; }
        [StringLength(200)]
        public string DateCreated { get; set; }
        [StringLength(200)]
        public string User { get; set; }
    }
}
