using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayInsuranceCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryId")]
        public int NCategoryId { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Column("X_CategoryName")]
        [StringLength(150)]
        public string XCategoryName { get; set; }
        [Column("N_TypeId")]
        public int? NTypeId { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
    }
}
