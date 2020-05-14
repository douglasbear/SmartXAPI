using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Cor_Correspondence")]
    public partial class CorCorrespondence
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_CorrespondenceId")]
        public int NCorrespondenceId { get; set; }
        [Column("X_CorrespondenceNo")]
        [StringLength(50)]
        public string XCorrespondenceNo { get; set; }
        [Column("D_Date", TypeName = "smalldatetime")]
        public DateTime? DDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_Notes")]
        public string XNotes { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
    }
}
