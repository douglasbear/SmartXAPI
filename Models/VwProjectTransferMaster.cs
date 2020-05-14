using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectTransferMaster
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [StringLength(8000)]
        public string Date { get; set; }
        [Column("Project_From")]
        public string ProjectFrom { get; set; }
        [Column("Project_To")]
        public string ProjectTo { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("N_ProjectIDFrom")]
        public int? NProjectIdfrom { get; set; }
        [Column("N_ProjectIDTo")]
        public int? NProjectIdto { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
