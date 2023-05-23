using System.ComponentModel.DataAnnotations;

namespace PustokBackTask.ViewModels
{
    public class MemberLoginViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [Required]
        [MaxLength (20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
