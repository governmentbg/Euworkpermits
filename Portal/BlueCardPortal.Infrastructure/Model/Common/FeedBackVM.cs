using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Common
{
    public class FeedBackVM
    {
        [Required(ErrorMessage = "Изборът на поле „Съобщение за“ е задължително.")]
        [Display(Name = "Съобщение за")]
        public long? TypeId { get; set; }

        [StringLength(100, ErrorMessage = "Име и фамилия не могат да съдържат повече от 100 символа.")]
        [RegularExpression(@"^[а-яА-Яa-zA-Z ]*$", ErrorMessage = "Име и фамилия трябва да съдържат само букви.")]
        [Display(Name = "Име и фамилия")]
        public string? Name { get; set; }

        [StringLength(100, ErrorMessage = "Електронната поща не трябва да надвишава 100 символа.")]
        [RegularExpression(@"^[\w\-!#$%&'*+/=?^`{|}~.""]+@([\w]+[.-]?)+[\w]\.[\w]+$", ErrorMessage = "Невалидна електронна поща.")]
        [Display(Name = "Eлектронна поща")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Текстът в полето „Описание“ е задължителен.")]
        [StringLength(4000, ErrorMessage = "Текстът в полето „Описание“ не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Описание")]
        public string? Message { get; set; }

        public string? Captcha { get; set; }
    }
}
