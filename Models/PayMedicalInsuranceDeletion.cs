using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_MedicalInsuranceDeletion")]
    public partial class PayMedicalInsuranceDeletion
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int NFnYearId { get; set; }
        [Key]
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
    }
}
