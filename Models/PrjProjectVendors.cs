using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectVendors")]
    public partial class PrjProjectVendors
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectVendorID")]
        public int? NProjectVendorId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_VendorAmt", TypeName = "money")]
        public decimal? NVendorAmt { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NProjectId))]
        public virtual PrjProjectMaster NProject { get; set; }
        [ForeignKey(nameof(NVendorId))]
        public virtual PrjVendor NVendor { get; set; }
    }
}
