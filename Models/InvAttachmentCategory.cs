using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_AttachmentCategory")]
    public partial class InvAttachmentCategory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_Category")]
        [StringLength(500)]
        public string XCategory { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(50)]
        public string XCategoryCode { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
    }
}
