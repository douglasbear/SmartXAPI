using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvReceivableStock
    {
        public string LocationCodeTo { get; set; }
        public string LocationNameTo { get; set; }
        [Column("LocationIDTo")]
        public int? LocationIdto { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_ReceivableId")]
        public int NReceivableId { get; set; }
        [StringLength(50)]
        public string MemoNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("LocationIDFrom")]
        public int? LocationIdfrom { get; set; }
        public string LocationCodeFrom { get; set; }
        public string LocationNameFrom { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
    }
}
