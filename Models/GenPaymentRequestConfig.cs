using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_PaymentRequestConfig")]
    public partial class GenPaymentRequestConfig
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ConfigID")]
        public int NConfigId { get; set; }
        [Column("X_RequestType")]
        public string XRequestType { get; set; }
        [Column("X_Notes")]
        public string XNotes { get; set; }
        [Column("N_ParentMenuID")]
        public int? NParentMenuId { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("N_LedgerID")]
        public int? NLedgerId { get; set; }
        [Column("N_LandingForm")]
        public int? NLandingForm { get; set; }
        public bool? IsGridFill { get; set; }
    }
}
