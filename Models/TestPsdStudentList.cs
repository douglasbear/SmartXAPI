using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("test__PSD_StudentList")]
    public partial class TestPsdStudentList
    {
        [Column("X_Status")]
        [StringLength(200)]
        public string XStatus { get; set; }
    }
}
