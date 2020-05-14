using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchParentLetterRpt
    {
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_ClassTypeID")]
        public int NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Column("N_SaleAmt", TypeName = "money")]
        public decimal? NSaleAmt { get; set; }
        [Column("N_ReceivedAmt", TypeName = "money")]
        public decimal NReceivedAmt { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal NDiscount { get; set; }
        [Column("N_BalanceAmt", TypeName = "money")]
        public decimal? NBalanceAmt { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(100)]
        public string XInvoiceNo { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
    }
}
