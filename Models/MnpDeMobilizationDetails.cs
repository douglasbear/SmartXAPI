using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_DeMobilizationDetails")]
    public partial class MnpDeMobilizationDetails
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_DeMobilizationID")]
        public int NDeMobilizationId { get; set; }
        [Key]
        [Column("N_DeMobilizationDetailsID")]
        public int NDeMobilizationDetailsId { get; set; }
        [Column("N_MobilizationDetailsID")]
        public int NMobilizationDetailsId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_MaintenanceID")]
        public int NMaintenanceId { get; set; }
        [Column("N_DailyRate", TypeName = "money")]
        public decimal? NDailyRate { get; set; }
    }
}
