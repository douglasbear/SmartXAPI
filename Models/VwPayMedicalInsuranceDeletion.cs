using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayMedicalInsuranceDeletion
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int NFnYearId { get; set; }
        [Column("N_DeletionID")]
        public int NDeletionId { get; set; }
        [Column("X_DeletionCode")]
        [StringLength(500)]
        public string XDeletionCode { get; set; }
        [Column("N_MedicalInsID")]
        public int NMedicalInsId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Required]
        [Column("X_InsuranceCode")]
        [StringLength(50)]
        public string XInsuranceCode { get; set; }
        [Required]
        [Column("X_InsuranceName")]
        [StringLength(100)]
        public string XInsuranceName { get; set; }
    }
}
