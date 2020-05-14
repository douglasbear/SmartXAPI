using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDmsReminderCategoryDetails
    {
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_Days")]
        public int? NDays { get; set; }
        [Column("X_Recipient")]
        [StringLength(100)]
        public string XRecipient { get; set; }
        [Column("X_CategoryCode")]
        [StringLength(100)]
        public string XCategoryCode { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
    }
}
