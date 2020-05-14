using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwRstTenentInvoice
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BatchID")]
        public int NBatchId { get; set; }
        [Column("X_BatchCode")]
        [StringLength(50)]
        public string XBatchCode { get; set; }
        [Column("D_ProcessDate", TypeName = "smalldatetime")]
        public DateTime? DProcessDate { get; set; }
        [Column("N_Month")]
        public int? NMonth { get; set; }
        [Column("N_Year")]
        public int? NYear { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_TenentID")]
        public int NTenentId { get; set; }
        [Column("X_TenentCode")]
        [StringLength(50)]
        public string XTenentCode { get; set; }
        [Column("X_TenentName")]
        [StringLength(400)]
        public string XTenentName { get; set; }
        [Column("X_RoomNo")]
        [StringLength(50)]
        public string XRoomNo { get; set; }
        [Column("N_floorID")]
        public int? NFloorId { get; set; }
        [Column("X_MonthCode")]
        [StringLength(9)]
        public string XMonthCode { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
    }
}
