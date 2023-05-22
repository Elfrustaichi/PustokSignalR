using System.ComponentModel.DataAnnotations;

namespace PustokBackTask.Models
{
    public class Setting
    {
        [MaxLength(25)]
        public string Key { get; set; }
        [MaxLength(300)]
        public string Value { get; set; }
    }
}
