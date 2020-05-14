using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvVendorProjectsDisp
    {
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("Project Name")]
        [StringLength(100)]
        public string ProjectName { get; set; }
        [Column("Project Code")]
        [StringLength(50)]
        public string ProjectCode { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("Vendor Code")]
        [StringLength(50)]
        public string VendorCode { get; set; }
        [Column("Vendor Name")]
        [StringLength(100)]
        public string VendorName { get; set; }
    }
}
