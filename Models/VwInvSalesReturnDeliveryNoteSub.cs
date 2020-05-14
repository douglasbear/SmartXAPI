using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesReturnDeliveryNoteSub
    {
        [Column("N_RetQty")]
        public int NRetQty { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_DeliveryNoteDetailsID")]
        public int NDeliveryNoteDetailsId { get; set; }
        [Column("N_ReturnCost", TypeName = "money")]
        public decimal? NReturnCost { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
    }
}
