using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_RequestQuotation")]
    public partial class InvRequestQuotation
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_RequestId")]
        public int NRequestId { get; set; }
        [Column("X_RequestNo")]
        [StringLength(50)]
        public string XRequestNo { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("D_RequestDate", TypeName = "smalldatetime")]
        public DateTime? DRequestDate { get; set; }
        [Column("N_VendorId")]
        public int? NVendorId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
