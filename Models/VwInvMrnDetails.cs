using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvMrnDetails
    {
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_MRNID")]
        public int NMrnid { get; set; }
        [StringLength(50)]
        public string OrderNo { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_MRNDate", TypeName = "datetime")]
        public DateTime? DMrndate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Required]
        [Column("X_VendorInvoice")]
        [StringLength(50)]
        public string XVendorInvoice { get; set; }
        [Required]
        [Column("X_VendorDeliveryNote")]
        [StringLength(15)]
        public string XVendorDeliveryNote { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Required]
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
    }
}
