﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGenFollowUp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("N_RefTypeID")]
        [StringLength(10)]
        public string NRefTypeId { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_EntryFrom")]
        [StringLength(50)]
        public string XEntryFrom { get; set; }
        [Column("B_IsComplete")]
        public bool BIsComplete { get; set; }
        [Column("D_CompleteDate", TypeName = "datetime")]
        public DateTime? DCompleteDate { get; set; }
        [Column("N_CompleteUserID")]
        public int? NCompleteUserId { get; set; }
        [Required]
        [StringLength(60)]
        public string UserName { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("B_IsEnquiry")]
        public bool BIsEnquiry { get; set; }
    }
}
