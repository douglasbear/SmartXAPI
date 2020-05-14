using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ServiceAccessories")]
    public partial class InvServiceAccessories
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ServiceID")]
        public int NServiceId { get; set; }
        [Key]
        [Column("N_ServiceDetailsID")]
        public int NServiceDetailsId { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("X_SerialNo")]
        [StringLength(100)]
        public string XSerialNo { get; set; }
        [Column("N_Qty")]
        public int? NQty { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
