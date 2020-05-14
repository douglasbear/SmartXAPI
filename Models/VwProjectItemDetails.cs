using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectItemDetails
    {
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [StringLength(800)]
        public string Description { get; set; }
        [Column("N_CurrentStock")]
        [StringLength(30)]
        public string NCurrentStock { get; set; }
        [Column("Product Code")]
        [StringLength(100)]
        public string ProductCode { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemUnitID")]
        public int NItemUnitId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
