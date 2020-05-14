using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCorrespondenceDashboard
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CorrespondenceId")]
        public int NCorrespondenceId { get; set; }
        [Column("X_CorrespondenceNo")]
        [StringLength(50)]
        public string XCorrespondenceNo { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_CorDetailsID")]
        public int NCorDetailsId { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("X_AttachmentID")]
        public int XAttachmentId { get; set; }
        [Required]
        [Column("D_DetailDate")]
        [StringLength(10)]
        public string DDetailDate { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Required]
        [Column("X_CurDetailsNo")]
        [StringLength(50)]
        public string XCurDetailsNo { get; set; }
        [Required]
        [Column("D_DetailEntryDate")]
        [StringLength(8000)]
        public string DDetailEntryDate { get; set; }
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Required]
        [Column("X_Notes")]
        public string XNotes { get; set; }
        [Column("Client_Description")]
        [StringLength(50)]
        public string ClientDescription { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
