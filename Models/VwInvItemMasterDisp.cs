using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemMasterDisp
    {
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Item Code")]
        [StringLength(100)]
        public string ItemCode { get; set; }
        [Column("Item Name")]
        [StringLength(800)]
        public string ItemName { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("Item Class")]
        [StringLength(50)]
        public string ItemClass { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_PartNo")]
        [StringLength(250)]
        public string XPartNo { get; set; }
        [Column("X_Barcode")]
        [StringLength(50)]
        public string XBarcode { get; set; }
        [Column("B_IsIMEI")]
        public bool? BIsImei { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_CategoryID1")]
        public int? NCategoryId1 { get; set; }
        [Required]
        [Column("X_FullLengthDescription")]
        [StringLength(2000)]
        public string XFullLengthDescription { get; set; }
    }
}
