using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_TransferStock")]
    public partial class InvTransferStock
    {
        public InvTransferStock()
        {
            InvReceivableStock = new HashSet<InvReceivableStock>();
            InvTransferStockDetails = new HashSet<InvTransferStockDetails>();
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
        [Column("N_LocationIDFrom")]
        public int? NLocationIdfrom { get; set; }
        [Column("N_LocationIDTo")]
        public int? NLocationIdto { get; set; }
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
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.InvTransferStock))]
        public virtual AccCompany NCompany { get; set; }
        [InverseProperty("NTransfer")]
        public virtual ICollection<InvReceivableStock> InvReceivableStock { get; set; }
        [InverseProperty("NTransfer")]
        public virtual ICollection<InvTransferStockDetails> InvTransferStockDetails { get; set; }
    }
}
