using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvWarrantyDetails
    {
        [Column("N_WarrantyID")]
        public int? NWarrantyId { get; set; }
        [Column("X_Warranty")]
        [StringLength(100)]
        public string XWarranty { get; set; }
        [Column("N_WarrantyTypeID")]
        public int? NWarrantyTypeId { get; set; }
        [Column("X_WarrantyCode")]
        [StringLength(50)]
        public string XWarrantyCode { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DurationID")]
        public double? NDurationId { get; set; }
        [Column("SalesID")]
        public int SalesId { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_ItemId")]
        public int NItemId { get; set; }
        [Column("N_ContractID")]
        public int NContractId { get; set; }
        [Column("N_SalesDetailsID")]
        public int? NSalesDetailsId { get; set; }
    }
}
