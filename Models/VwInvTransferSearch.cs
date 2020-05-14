using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvTransferSearch
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("Reference No")]
        [StringLength(50)]
        public string ReferenceNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("N_LocationFrom")]
        public int? NLocationFrom { get; set; }
        [Column("N_LocationTo")]
        public int? NLocationTo { get; set; }
        [Required]
        [Column("Site From")]
        public string SiteFrom { get; set; }
        [Required]
        [Column("Site To")]
        public string SiteTo { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("Branch From")]
        public int BranchFrom { get; set; }
        [Column("Branch To")]
        public int BranchTo { get; set; }
    }
}
