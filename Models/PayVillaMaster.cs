using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_VillaMaster")]
    public partial class PayVillaMaster
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VillaId")]
        public int NVillaId { get; set; }
        [Column("X_VillaNo")]
        [StringLength(100)]
        public string XVillaNo { get; set; }
        [Column("X_VillaName")]
        [StringLength(100)]
        public string XVillaName { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.PayVillaMaster))]
        public virtual AccCompany NCompany { get; set; }
    }
}
