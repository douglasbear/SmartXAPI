using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeProcessing")]
    public partial class SchFeeProcessing
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int? NAcYearId { get; set; }
        [Required]
        [Column("X_ProcessingID")]
        [StringLength(50)]
        public string XProcessingId { get; set; }
        [Column("D_ProcessingDate", TypeName = "datetime")]
        public DateTime? DProcessingDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Key]
        [Column("N_ProcessingID")]
        public int NProcessingId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
