using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRfqgeneratingDetails
    {
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyNo")]
        [StringLength(50)]
        public string XPkeyNo { get; set; }
        [Column("N_ReqstDetailsID")]
        public int NReqstDetailsId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
    }
}
