using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_WarehouseMaster")]
    public partial class InvWarehouseMaster
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_WarehouseID")]
        public int NWarehouseId { get; set; }
        [Column("X_WarehouseName")]
        [StringLength(50)]
        public string XWarehouseName { get; set; }
        [Column("X_WarehouseCode")]
        [StringLength(50)]
        public string XWarehouseCode { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.InvWarehouseMaster))]
        public virtual AccCompany NCompany { get; set; }
    }
}
