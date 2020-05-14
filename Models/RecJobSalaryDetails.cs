using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rec_JobSalaryDetails")]
    public partial class RecJobSalaryDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
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
        [Column("N_RevNo")]
        public int? NRevNo { get; set; }
    }
}
