using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_StudentHouse")]
    public partial class SchStudentHouse
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_HouseID")]
        public int NHouseId { get; set; }
        [Required]
        [Column("X_HouseCode")]
        [StringLength(50)]
        public string XHouseCode { get; set; }
        [Column("X_HouseName")]
        [StringLength(50)]
        public string XHouseName { get; set; }
        [Column("X_HouseDesc")]
        [StringLength(50)]
        public string XHouseDesc { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
