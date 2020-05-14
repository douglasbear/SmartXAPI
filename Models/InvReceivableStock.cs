using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ReceivableStock")]
    public partial class InvReceivableStock
    {
        public InvReceivableStock()
        {
            InvReceivableStockDetails = new HashSet<InvReceivableStockDetails>();
        }

        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_ReceivableId")]
        public int NReceivableId { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_ReceivableDate", TypeName = "smalldatetime")]
        public DateTime? DReceivableDate { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.InvReceivableStock))]
        public virtual AccCompany NCompany { get; set; }
        [ForeignKey(nameof(NTransferId))]
        [InverseProperty(nameof(InvTransferStock.InvReceivableStock))]
        public virtual InvTransferStock NTransfer { get; set; }
        [InverseProperty("NReceivable")]
        public virtual ICollection<InvReceivableStockDetails> InvReceivableStockDetails { get; set; }
    }
}
