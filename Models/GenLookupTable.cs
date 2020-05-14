using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_LookupTable")]
    public partial class GenLookupTable
    {
        public GenLookupTable()
        {
            InvServiceBooking = new HashSet<InvServiceBooking>();
        }

        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_Name_Ar")]
        [StringLength(250)]
        public string XNameAr { get; set; }
        [Column("X_Description")]
        [StringLength(200)]
        public string XDescription { get; set; }

        [InverseProperty("NAssigned")]
        public virtual ICollection<InvServiceBooking> InvServiceBooking { get; set; }
    }
}
