using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaAppointmentDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Column("User_COde")]
        [StringLength(50)]
        public string UserCode { get; set; }
        [StringLength(100)]
        public string UserName { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("X_Name")]
        [StringLength(100)]
        public string XName { get; set; }
        [Column("X_Email")]
        [StringLength(50)]
        public string XEmail { get; set; }
        [Column("N_RefTypeID")]
        [StringLength(10)]
        public string NRefTypeId { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
    }
}
