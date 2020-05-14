using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_DeliveryDispatchDetails")]
    public partial class InvDeliveryDispatchDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DispatchID")]
        public int NDispatchId { get; set; }
        [Key]
        [Column("N_DispatchDetailsID")]
        public int NDispatchDetailsId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_SalesDetailsID")]
        public int NSalesDetailsId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Location")]
        [StringLength(200)]
        public string XLocation { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("B_Dispatch")]
        public bool? BDispatch { get; set; }
    }
}
