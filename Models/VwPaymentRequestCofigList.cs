using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaymentRequestCofigList
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ConfigID")]
        public int NConfigId { get; set; }
        [Column("X_RequestType")]
        public string XRequestType { get; set; }
        [Required]
        [Column("X_Notes")]
        public string XNotes { get; set; }
        [Column("N_MenuID")]
        public int? NMenuId { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
    }
}
