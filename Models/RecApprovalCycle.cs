﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rec_ApprovalCycle")]
    public partial class RecApprovalCycle
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Key]
        [Column("N_ApprovalID")]
        public int NApprovalId { get; set; }
        [Column("X_ApprovalCode")]
        [StringLength(50)]
        public string XApprovalCode { get; set; }
        [Column("X_ApprovalName")]
        [StringLength(50)]
        public string XApprovalName { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
    }
}
