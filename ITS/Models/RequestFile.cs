using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KobiAsITS.Models
{
    public class RequestFile
    {
        [Key]
        public int Id { get; set; }
        public string Uuid { get; set; }
        public int RequestId { get; set; }
        public string FilePath { get; set; }
        public DateTime CreateDate { get; set; }

        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; }
       

    }
}
