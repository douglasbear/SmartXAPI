using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPendingPurchasesRpt
    {
        [Column("N_MRNID")]
        public int NMrnid { get; set; }
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Column("D_MRNDate", TypeName = "datetime")]
        public DateTime? DMrndate { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        public int? Expr1 { get; set; }
        [Column("N_MRNDetailsID")]
        public int NMrndetailsId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("X_Description")]
        public string XDescription { get; set; }
    }
}
