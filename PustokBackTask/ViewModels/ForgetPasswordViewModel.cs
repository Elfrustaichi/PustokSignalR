using System.ComponentModel.DataAnnotations;

namespace PustokBackTask.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
