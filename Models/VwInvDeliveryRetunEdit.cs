using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDeliveryRetunEdit
    {
        [Column("X_DebitNoteNo")]
        [StringLength(50)]
        public string XDebitNoteNo { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_TotalRetQty")]
        public int NTotalRetQty { get; set; }
        [Column("N_RetQty")]
        public int? NRetQty { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DebitNoteId")]
        public int NDebitNoteId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("X_ReturnType")]
        [StringLength(25)]
        public string XReturnType { get; set; }
        [Column("X_ReturnRemarks")]
        [StringLength(150)]
        public string XReturnRemarks { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_TotalReturnAmount", TypeName = "money")]
        public decimal? NTotalReturnAmount { get; set; }
        [Column("N_DeliveryNoteId")]
        public int NDeliveryNoteId { get; set; }
        [Column("N_DeliveryNoteDetailsID")]
        public int NDeliveryNoteDetailsId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_TotalReturn")]
        public int? NTotalReturn { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_BaseQty")]
        public double? NBaseQty { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_BaseUnit")]
        [StringLength(500)]
        public string XBaseUnit { get; set; }
        [Column("N_ReturnCost", TypeName = "money")]
        public decimal? NReturnCost { get; set; }
        [Column("N_RetAmount")]
        public int NRetAmount { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_Sprice", TypeName = "money")]
        public decimal? NSprice { get; set; }
    }
}
