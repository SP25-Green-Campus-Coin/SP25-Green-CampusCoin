using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusCoin.Models
{
    public partial class Expenses
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public double Bills { get; set; }

        [Required]
        public double Food { get; set; }

        [Required]
        public double Auto { get; set; }

        [Required]
        public double Entertainment { get; set; }

        [Required]
        public double Investments { get; set; }

        [Required]
        public double Misc { get; set; }
    }
}
