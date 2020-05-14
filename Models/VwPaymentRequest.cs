using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaymentRequest
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_RequestID")]
        public int? NRequestId { get; set; }
        [Column("X_RequestCode")]
        [StringLength(100)]
        public string XRequestCode { get; set; }
        [Column("X_RequestType")]
        [StringLength(100)]
        public string XRequestType { get; set; }
        [Column("N_ApprovedUserID")]
        public int? NApprovedUserId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("D_DelDate", TypeName = "datetime")]
        public DateTime? DDelDate { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal? NNetAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_RequestTypeID")]
        public int? NRequestTypeId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
    }
}
