using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPendingGrnRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_POQty")]
        public double? NPoqty { get; set; }
        [Column("N_MRNQty")]
        public double? NMrnqty { get; set; }
        [Column("N_PQty")]
        public double? NPqty { get; set; }
        [Required]
        [Column("N_Status")]
        [StringLength(7)]
        public string NStatus { get; set; }
        [Column("X_VendorCode")]
        [StringLength(50)]
        public string XVendorCode { get; set; }
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_POrderDetailsID")]
        [StringLength(100)]
        public string NPorderDetailsId { get; set; }
        [Column("N_POrderID")]
        public int? NPorderId { get; set; }
        [Column("X_FreeDescription")]
        [StringLength(100)]
        public string XFreeDescription { get; set; }
    }
}
