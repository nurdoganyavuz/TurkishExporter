using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KobiAsITS.Models
{
    public class RolePermission
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        [ForeignKey("PermissionId")]
        public virtual Permission Permission{ get; set; }
        public virtual Role Role { get; set; }

    }
}
