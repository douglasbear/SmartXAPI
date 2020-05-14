using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCorrespondDashboard
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
        [Column("Client Name")]
        [StringLength(50)]
        public string ClientName { get; set; }
        [Column("X_ClientCode")]
        [StringLength(25)]
        public string XClientCode { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_CorDetailsID")]
        public int? NCorDetailsId { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("X_AttachmentID")]
        public int? XAttachmentId { get; set; }
        [Column("D_DetailDate")]
        [StringLength(8000)]
        public string DDetailDate { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("X_CurDetailsNo")]
        [StringLength(50)]
        public string XCurDetailsNo { get; set; }
        [Column("D_DetailEntryDate")]
        [StringLength(8000)]
        public string DDetailEntryDate { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(6)]
        public string XStatus { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("X_Notes")]
        public string XNotes { get; set; }
    }
}
