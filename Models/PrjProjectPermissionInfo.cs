using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_PRojectPermissionInfo")]
    public partial class PrjProjectPermissionInfo
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PermissionID")]
        public int NPermissionId { get; set; }
        [Column("X_PermissionCode")]
        [StringLength(50)]
        public string XPermissionCode { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_PermissionNo")]
        [StringLength(100)]
        public string XPermissionNo { get; set; }
        [Column("D_Startdate", TypeName = "datetime")]
        public DateTime DStartdate { get; set; }
        [Column("D_Enddate", TypeName = "datetime")]
        public DateTime DEnddate { get; set; }
        [Column("X_Permits")]
        [StringLength(100)]
        public string XPermits { get; set; }
        [Column("N_TrenchID")]
        public int? NTrenchId { get; set; }
        [Column("N_MillingID")]
        public int? NMillingId { get; set; }
        [Column("N_CompactionID")]
        public int? NCompactionId { get; set; }
        [Column("N_AsphaltID")]
        public int? NAsphaltId { get; set; }
        [Column("X_Penalty")]
        [StringLength(100)]
        public string XPenalty { get; set; }
        [Column("N_PenaltyAmt")]
        public int NPenaltyAmt { get; set; }
        [Column("X_Location")]
        [StringLength(100)]
        public string XLocation { get; set; }
        [Column("X_Length")]
        [StringLength(10)]
        public string XLength { get; set; }
        [Column("X_Team")]
        [StringLength(100)]
        public string XTeam { get; set; }
        [Column("N_ConsultantID")]
        public int? NConsultantId { get; set; }
        [Column("N_OperationID")]
        public int? NOperationId { get; set; }
        [Column("N_BaladiyaID")]
        public int? NBaladiyaId { get; set; }
        [Column("N_AmanahID")]
        public int? NAmanahId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_PermitUpdate")]
        [StringLength(255)]
        public string XPermitUpdate { get; set; }
        [Column("X_Notes")]
        [StringLength(500)]
        public string XNotes { get; set; }
    }
}
