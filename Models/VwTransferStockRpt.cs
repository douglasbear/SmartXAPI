using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwTransferStockRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_ItemDiscAmt", TypeName = "money")]
        public decimal? NItemDiscAmt { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_TruckID")]
        public int? NTruckId { get; set; }
        [Column("X_TruckCode")]
        [StringLength(100)]
        public string XTruckCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_PlateNumber")]
        [StringLength(100)]
        public string XPlateNumber { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_UnitName")]
        [StringLength(500)]
        public string XUnitName { get; set; }
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("X_IssuedTo")]
        [StringLength(100)]
        public string XIssuedTo { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("N_LocationIDFrom")]
        public int? NLocationIdfrom { get; set; }
        [Column("N_LocationIDTo")]
        public int? NLocationIdto { get; set; }
        [Column("X_LocationFrom")]
        public string XLocationFrom { get; set; }
        [Column("X_LocationTo")]
        public string XLocationTo { get; set; }
    }
}
