using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSisdetails
    {
        [Column("N_TransferID")]
        public int? NTransferId { get; set; }
        [Column("N_TransferDetailsID")]
        public int? NTransferDetailsId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("N_MainSPrice", TypeName = "money")]
        public decimal? NMainSprice { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_BaseUnitID")]
        public int? NBaseUnitId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_UnitQty")]
        public double? NUnitQty { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        public double? Qty { get; set; }
        public double? RemaingQty { get; set; }
        [Column("N_ProcuredQty")]
        public double? NProcuredQty { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("N_PRSDetailsID")]
        public int NPrsdetailsId { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_DepQty")]
        public double? NDepQty { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_TruckID")]
        public int? NTruckId { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("X_TruckName")]
        [StringLength(100)]
        public string XTruckName { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
    }
}
