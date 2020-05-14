using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvPaymentRequestList
    {
        [Column("N_RequestID")]
        public int NRequestId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [Column("X_RequestCode")]
        [StringLength(50)]
        public string XRequestCode { get; set; }
        [Required]
        [Column("X_RequestType")]
        [StringLength(100)]
        public string XRequestType { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_PartyID")]
        public int? NPartyId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [StringLength(30)]
        public string NetAmount { get; set; }
        [Column("N_NextApprovalID")]
        public int? NNextApprovalId { get; set; }
    }
}
