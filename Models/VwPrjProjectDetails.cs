using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjProjectDetails
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(50)]
        public string XProjectCode { get; set; }
        [Column("X_ProjectDescr")]
        [StringLength(100)]
        public string XProjectDescr { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_InvoiceAmt", TypeName = "money")]
        public decimal? NInvoiceAmt { get; set; }
        [Column("N_PurchaseID")]
        public int NPurchaseId { get; set; }
        [Column("N_PurchaseType")]
        public int? NPurchaseType { get; set; }
        [Column("N_VendorID")]
        public int NVendorId { get; set; }
    }
}
