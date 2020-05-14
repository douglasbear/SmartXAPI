using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStockAdjustmentMasterDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_LoactionID")]
        public int? NLoactionId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("D_AdjustDate", TypeName = "datetime")]
        public DateTime? DAdjustDate { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
    }
}
