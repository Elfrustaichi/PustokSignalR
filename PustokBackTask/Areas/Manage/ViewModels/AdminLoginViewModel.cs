
using System.ComponentModel.DataAnnotations;


namespace PustokBackTask.Areas.Manage.ViewModels
{
    public class AdminLoginViewModel
    {
        
        [MaxLength(20)]
        [Required]
        public string Username { get; set; }
        [Required]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
