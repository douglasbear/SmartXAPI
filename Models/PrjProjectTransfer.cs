using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectTransfer")]
    public partial class PrjProjectTransfer
    {
        public PrjProjectTransfer()
        {
            PrjProjectTransferDetails = new HashSet<PrjProjectTransferDetails>();
        }

        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_ProjectIDFrom")]
        public int? NProjectIdfrom { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_PRSID")]
        public int? NPrsid { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_Processed")]
        public bool? NProcessed { get; set; }
        [Column("X_IssuedTo")]
        [StringLength(100)]
        public string XIssuedTo { get; set; }
        [Column("N_ProjectIDTo")]
        public int? NProjectIdto { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.PrjProjectTransfer))]
        public virtual AccCompany NCompany { get; set; }
        [InverseProperty("NTransfer")]
        public virtual ICollection<PrjProjectTransferDetails> PrjProjectTransferDetails { get; set; }
    }
}
