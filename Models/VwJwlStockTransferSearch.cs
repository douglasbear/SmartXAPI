using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlStockTransferSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_TransferID")]
        public int NTransferId { get; set; }
        [Column("Reference No")]
        [StringLength(50)]
        public string ReferenceNo { get; set; }
        [Column("N_LocationFrom")]
        public int? NLocationFrom { get; set; }
        [Column("N_LocationTo")]
        public int? NLocationTo { get; set; }
        [Column("Site From")]
        public string SiteFrom { get; set; }
        [Column("Site To")]
        public string SiteTo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
    }
}
