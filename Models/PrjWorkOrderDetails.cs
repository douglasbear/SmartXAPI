using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_WorkOrderDetails")]
    public partial class PrjWorkOrderDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_WorkOrderID")]
        public int? NWorkOrderId { get; set; }
        [Key]
        [Column("N_WorkOrderDetailsID")]
        public int NWorkOrderDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_PricePerc", TypeName = "money")]
        public decimal? NPricePerc { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_BOQID")]
        public int? NBoqid { get; set; }
        [Column("N_CostCategoryID")]
        public int? NCostCategoryId { get; set; }
        [Column("N_VATID")]
        public int? NVatid { get; set; }
        [Column("X_Code")]
        [StringLength(50)]
        public string XCode { get; set; }
        [Column("X_ShortDesc")]
        [StringLength(200)]
        public string XShortDesc { get; set; }
        [Column("X_LongDesc")]
        [StringLength(2000)]
        public string XLongDesc { get; set; }
    }
}
