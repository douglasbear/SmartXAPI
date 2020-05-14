using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_ReservationPayment_Detail")]
    public partial class SchReservationPaymentDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PaymentID")]
        public int NPaymentId { get; set; }
        [Key]
        [Column("N_PaymentDetailsID")]
        public int NPaymentDetailsId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("B_IsNewStudent")]
        public bool? BIsNewStudent { get; set; }
        [Column("N_Regid")]
        public int? NRegid { get; set; }
    }
}
