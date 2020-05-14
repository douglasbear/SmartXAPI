using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvTranferDetailedRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("D_TransferDate", TypeName = "smalldatetime")]
        public DateTime? DTransferDate { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        public string LocationFrom { get; set; }
        public string LocationTo { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_SPrice", TypeName = "money")]
        public decimal? NSprice { get; set; }
        [Column("N_TransferDetailsID")]
        public int NTransferDetailsId { get; set; }
        [Column("N_TransferId")]
        public int NTransferId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
    }
}
