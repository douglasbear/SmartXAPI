using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCorrTransDetails
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CorrespondenceId")]
        public int NCorrespondenceId { get; set; }
        [Column("X_CorrespondenceNo")]
        [StringLength(50)]
        public string XCorrespondenceNo { get; set; }
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
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("X_AttachmentID")]
        public int? XAttachmentId { get; set; }
        [Column("X_CurDetailsNo")]
        [StringLength(50)]
        public string XCurDetailsNo { get; set; }
        [Column("X_ClientCode")]
        [StringLength(25)]
        public string XClientCode { get; set; }
        [Column("Client Name")]
        [StringLength(50)]
        public string ClientName { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_UserCategoryID")]
        public int? NUserCategoryId { get; set; }
    }
}
