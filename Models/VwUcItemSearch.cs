using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwUcItemSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("Product Code")]
        [StringLength(100)]
        public string ProductCode { get; set; }
        [StringLength(800)]
        public string Description { get; set; }
        [Column("Description_Ar")]
        [StringLength(800)]
        public string DescriptionAr { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("Part No")]
        [StringLength(250)]
        public string PartNo { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Required]
        [StringLength(100)]
        public string Category2 { get; set; }
        [Column("CategoryID1")]
        public int? CategoryId1 { get; set; }
        [Column("CategoryID2")]
        public int? CategoryId2 { get; set; }
    }
}
