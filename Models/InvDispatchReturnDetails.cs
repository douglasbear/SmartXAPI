using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_DispatchReturnDetails")]
    public partial class InvDispatchReturnDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DispatchReturnId")]
        public int? NDispatchReturnId { get; set; }
        [Key]
        [Column("N_DispatchReturnDetailsID")]
        public int NDispatchReturnDetailsId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_MainItemID")]
        public int? NMainItemId { get; set; }
        [Column("N_MainQty")]
        public double? NMainQty { get; set; }
        [Column("N_DispatchReqID")]
        public int? NDispatchReqId { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_QtyDisplay")]
        public double? NQtyDisplay { get; set; }
        [Column("X_ItemRemarks")]
        [StringLength(250)]
        public string XItemRemarks { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_ReturnCost", TypeName = "money")]
        public decimal? NReturnCost { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_RsID")]
        public int? NRsId { get; set; }
        [Column("X_RsNo")]
        [StringLength(15)]
        public string XRsNo { get; set; }
        [Column("N_RsDetailsID")]
        public int? NRsDetailsId { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_UnitCost", TypeName = "money")]
        public decimal? NUnitCost { get; set; }
        [Column("N_DispatchDetailsID")]
        public int? NDispatchDetailsId { get; set; }
    }
}
