using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_MobilizationDetails")]
    public partial class MnpMobilizationDetails
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Key]
        [Column("N_MobilizationDetailsID")]
        public int NMobilizationDetailsId { get; set; }
        [Column("N_MaintenanceID")]
        public int? NMaintenanceId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_DailyRate", TypeName = "money")]
        public decimal? NDailyRate { get; set; }
    }
}
