using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRstTenentInvoices
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_TenentID")]
        public int NTenentId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_TenentDetailID")]
        public int? NTenentDetailId { get; set; }
        [Column("X_TenentName")]
        [StringLength(400)]
        public string XTenentName { get; set; }
        [Column("X_PhoneNo")]
        [StringLength(50)]
        public string XPhoneNo { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_ExtensionCode")]
        [StringLength(10)]
        public string XExtensionCode { get; set; }
        [Column("n_BatchID")]
        public int? NBatchId { get; set; }
    }
}
