using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KobiAsITS.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string Uuid { get; set; }
        public int UserId { get; set; }
        public string RequestDescription { get; set; }
        public string RequestTitle { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public byte Priority { get; set; }
        public byte Status { get; set; }
        public int RequestProgressStatus { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public ICollection<RequestFile> RequestFiles { get; set; }
        public ICollection<Process> Process { get; set; }



    }
}
