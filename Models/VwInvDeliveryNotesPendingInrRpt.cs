using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDeliveryNotesPendingInrRpt
    {
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("D_PRSDate", TypeName = "datetime")]
        public DateTime? DPrsdate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SOQty")]
        public double? NSoqty { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("N_deliveredQty")]
        public double? NDeliveredQty { get; set; }
        [Column("N_ReqQty")]
        public double? NReqQty { get; set; }
        [Column("N_BalanceQty")]
        public double? NBalanceQty { get; set; }
        [Column("X_OrderNo")]
        [StringLength(50)]
        public string XOrderNo { get; set; }
        [Column("D_OrderDate", TypeName = "smalldatetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
    }
}
