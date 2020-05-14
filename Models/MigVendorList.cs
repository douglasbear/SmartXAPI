using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_VendorList")]
    public partial class MigVendorList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Vendor_Code")]
        [StringLength(200)]
        public string VendorCode { get; set; }
        [Column("Vendor_Name")]
        [StringLength(300)]
        public string VendorName { get; set; }
        [Column("Contact_Person")]
        [StringLength(300)]
        public string ContactPerson { get; set; }
        [Column("Contact_Number")]
        [StringLength(100)]
        public string ContactNumber { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [StringLength(100)]
        public string Currency { get; set; }
        [Column("Pending_Inv")]
        [StringLength(100)]
        public string PendingInv { get; set; }
        [Column("INV_DATE", TypeName = "date")]
        public DateTime? InvDate { get; set; }
        [Column("INV_DUE_AMOUNT")]
        [StringLength(100)]
        public string InvDueAmount { get; set; }
    }
}
