using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJobsalaryDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_SalaryDetailID")]
        public int NSalaryDetailId { get; set; }
        [Column("N_CandidateID")]
        public int NCandidateId { get; set; }
        [Required]
        [Column("X_CandidateCode")]
        [StringLength(20)]
        public string XCandidateCode { get; set; }
        [Column("N_PayCodeID")]
        public int NPayCodeId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_Entrydate", TypeName = "date")]
        public DateTime DEntrydate { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(3)]
        public string XType { get; set; }
    }
}
