using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_TaxType")]
    public partial class AccTaxType
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Required]
        [Column("X_TypeName")]
        [StringLength(30)]
        public string XTypeName { get; set; }
        [Column("X_MenuCaption")]
        [StringLength(50)]
        public string XMenuCaption { get; set; }
        [Column("X_ScreenCaption")]
        [StringLength(50)]
        public string XScreenCaption { get; set; }
        [Column("X_RepPathCaption")]
        [StringLength(30)]
        public string XRepPathCaption { get; set; }
    }
}
