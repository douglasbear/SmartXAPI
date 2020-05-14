using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPurchaseOrderDisp
    {
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Required]
        [StringLength(2)]
        public string Type { get; set; }
        [StringLength(50)]
        public string VendorCode { get; set; }
        [StringLength(100)]
        public string Vendor { get; set; }
        [Column("VendorID")]
        public int VendorId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [Column("Quotation No")]
        [StringLength(40)]
        public string QuotationNo { get; set; }
    }
}
