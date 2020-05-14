using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_WorkFlowCatalog")]
    public partial class InvWorkFlowCatalog
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ScreenID")]
        public int NScreenId { get; set; }
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_SRNo")]
        public int NSrno { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_UserLevel")]
        public int NUserLevel { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
    }
}
