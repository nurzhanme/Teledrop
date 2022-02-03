using System.ComponentModel.DataAnnotations;

namespace Teledrop.Models
{
    public class PhoneCodeViewModel
    {
        [Required]
        public string Phonenumber { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
