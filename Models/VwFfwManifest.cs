using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFfwManifest
    {
        [Column("N_ManifestID")]
        public int NManifestId { get; set; }
        [Column("N_LocFromID")]
        public int? NLocFromId { get; set; }
        [Column("X_LocFrom")]
        [StringLength(100)]
        public string XLocFrom { get; set; }
        [Column("N_LocToID")]
        public int? NLocToId { get; set; }
        [Column("X_LocTo")]
        [StringLength(100)]
        public string XLocTo { get; set; }
        [Column("X_ManifestNo")]
        [StringLength(50)]
        public string XManifestNo { get; set; }
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("D_ReferenceDate", TypeName = "smalldatetime")]
        public DateTime? DReferenceDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ModeTransactionID")]
        public int? NModeTransactionId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_TotAmt", TypeName = "money")]
        public decimal? NTotAmt { get; set; }
        [Required]
        [Column("X_AgentName")]
        [StringLength(100)]
        public string XAgentName { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Required]
        [Column("X_Driver")]
        [StringLength(100)]
        public string XDriver { get; set; }
        [Column("N_VehicleID")]
        public int NVehicleId { get; set; }
        [Column("N_agentID")]
        public int NAgentId { get; set; }
        [Column("N_TotAgentAmt", TypeName = "money")]
        public decimal NTotAgentAmt { get; set; }
    }
}
