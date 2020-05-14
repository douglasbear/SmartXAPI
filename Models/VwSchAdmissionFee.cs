using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchAdmissionFee
    {
        [Column("X_InvoiceNo")]
        [StringLength(100)]
        public string XInvoiceNo { get; set; }
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_InvoiceDueAmt", TypeName = "money")]
        public decimal? NInvoiceDueAmt { get; set; }
        [Required]
        [StringLength(8)]
        public string Status { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("B_IsRemoved")]
        public int? BIsRemoved { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column(TypeName = "money")]
        public decimal? Fee { get; set; }
        [Column("D_DateFrom", TypeName = "datetime")]
        public DateTime? DDateFrom { get; set; }
        [Column("D_DateTill", TypeName = "datetime")]
        public DateTime? DDateTill { get; set; }
        [Column("DFrom")]
        [StringLength(8000)]
        public string Dfrom { get; set; }
        [Column("DTo")]
        [StringLength(8000)]
        public string Dto { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Column("B_Paid")]
        public bool? BPaid { get; set; }
    }
}
