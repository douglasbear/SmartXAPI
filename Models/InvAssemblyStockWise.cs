using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_AssemblyStockWise")]
    public partial class InvAssemblyStockWise
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_DetailsID")]
        public int NDetailsId { get; set; }
        [Column("N_StockId")]
        public int? NStockId { get; set; }
        [Column("N_AssemblyId")]
        public int? NAssemblyId { get; set; }
        [Column("N_AssemblyDetailsId")]
        public int? NAssemblyDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NAssemblyId))]
        [InverseProperty(nameof(InvAssembly.InvAssemblyStockWise))]
        public virtual InvAssembly NAssembly { get; set; }
    }
}
