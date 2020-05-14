using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssTransferdetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("N_BookValue", TypeName = "money")]
        public decimal? NBookValue { get; set; }
        [Column("Branch To")]
        [StringLength(50)]
        public string BranchTo { get; set; }
        [Column("Cost Centre From")]
        [StringLength(100)]
        public string CostCentreFrom { get; set; }
        [Column("Cost Centre To")]
        [StringLength(100)]
        public string CostCentreTo { get; set; }
        [Column("Branch From")]
        [StringLength(50)]
        public string BranchFrom { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("X_Description")]
        [StringLength(150)]
        public string XDescription { get; set; }
        [Column("N_TransferID")]
        public int NTransferId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
    }
}
