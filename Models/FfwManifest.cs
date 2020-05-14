using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ffw_Manifest")]
    public partial class FfwManifest
    {
        [Column("N_CompanyId")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_ManifestID")]
        public int NManifestId { get; set; }
        [Column("X_ManifestNo")]
        [StringLength(50)]
        public string XManifestNo { get; set; }
        [Column("D_ReferenceDate", TypeName = "smalldatetime")]
        public DateTime? DReferenceDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ModeTransactionID")]
        public int? NModeTransactionId { get; set; }
        [Column("N_LocFromID")]
        public int? NLocFromId { get; set; }
        [Column("N_LocToID")]
        public int? NLocToId { get; set; }
        [Column("N_TotAmt", TypeName = "money")]
        public decimal? NTotAmt { get; set; }
        [Column("N_VehicleID")]
        public int? NVehicleId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_TotAgentAmt", TypeName = "money")]
        public decimal? NTotAgentAmt { get; set; }
    }
}
