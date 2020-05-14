using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrsitemDashboard
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string FileNo { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("N_Cost")]
        [StringLength(30)]
        public string NCost { get; set; }
        [Column(TypeName = "money")]
        public decimal? NetAmount { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("X_Status")]
        [StringLength(20)]
        public string XStatus { get; set; }
        [Required]
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Required]
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Required]
        [Column("X_VendorName")]
        [StringLength(100)]
        public string XVendorName { get; set; }
        [Column("N_MRNID")]
        public int NMrnid { get; set; }
        [Required]
        [Column("X_MRNNo")]
        [StringLength(50)]
        public string XMrnno { get; set; }
        [Required]
        [Column("X_SalesOrderNo")]
        [StringLength(50)]
        public string XSalesOrderNo { get; set; }
        [Column("N_PRSDetailsID")]
        public int NPrsdetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PrsCost", TypeName = "money")]
        public decimal? NPrsCost { get; set; }
        [Column("N_PRSQty")]
        [StringLength(30)]
        public string NPrsqty { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
    }
}
