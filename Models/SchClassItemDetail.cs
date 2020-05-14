using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ClassItemDetail")]
    public partial class SchClassItemDetail
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ClassID")]
        public int NClassId { get; set; }
        [Key]
        [Column("N_ItemID")]
        public int NItemId { get; set; }
    }
}
