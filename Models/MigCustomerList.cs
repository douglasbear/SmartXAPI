using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Mig_CustomerList")]
    public partial class MigCustomerList
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Customer_Code")]
        [StringLength(200)]
        public string CustomerCode { get; set; }
        [Column("Customer_Name")]
        [StringLength(300)]
        public string CustomerName { get; set; }
        [Column("Contact_Number")]
        [StringLength(300)]
        public string ContactNumber { get; set; }
        [Column("Sales_Executive")]
        [StringLength(300)]
        public string SalesExecutive { get; set; }
        [StringLength(100)]
        public string Branch { get; set; }
        [StringLength(100)]
        public string Address { get; set; }
        [Column("Pending_Inv")]
        [StringLength(100)]
        public string PendingInv { get; set; }
        [Column("INV_DATE", TypeName = "date")]
        public DateTime? InvDate { get; set; }
        [Column("INV_DUE_AMOUNT", TypeName = "money")]
        public decimal? InvDueAmount { get; set; }
        [Column("Credit_Limit")]
        [StringLength(100)]
        public string CreditLimit { get; set; }
    }
}
