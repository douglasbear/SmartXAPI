using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Dms_ReminderCategoryDetails")]
    public partial class DmsReminderCategoryDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_Days")]
        public int? NDays { get; set; }
        [Column("X_Recipient")]
        [StringLength(100)]
        public string XRecipient { get; set; }
        [Column("N_CategoryDetailsID")]
        public int? NCategoryDetailsId { get; set; }
    }
}
