using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KobiAsITS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Uuid { get; set; }
        public int DepartmentId { get; set; }
        public int RoleId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime CreateDate { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public Guid? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordExpired { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public ICollection<Request> Requests { get; set; }
        public ICollection<Process> Process { get; set; }

    }
}
