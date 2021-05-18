using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KobiAsITS.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Uuid { get; set; }

        [Required(ErrorMessage = "Zorunlu Alan")]
        public string DepartmentName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
