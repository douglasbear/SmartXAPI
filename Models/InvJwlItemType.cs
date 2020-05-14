using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Jwl_ItemType")]
    public partial class InvJwlItemType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TypeID")]
        public int NTypeId { get; set; }
        [Column("X_TypeName")]
        [StringLength(100)]
        public string XTypeName { get; set; }
    }
}
