using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class View2
    {
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Required]
        [Column("X_AdmissionNo")]
        [StringLength(25)]
        public string XAdmissionNo { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("n_ledgeridnew")]
        public int NLedgeridnew { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
