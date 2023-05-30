using PustokBackTask.Models;

namespace PustokBackTask.ViewModels
{
    public class AccountProfileViewModel
    {
        public ProfileEditViewModel Profile { get; set; }

        public List<Order> Orders { get; set; }
    }
}
