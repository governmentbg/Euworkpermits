using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    public class PersonIdDocumentListVM
    {
        public List<PersonIdDocumentVM> Items { get; set; } = new();
        [BCPersonIdDocumentList(ErrorMessage = "BCPersonIdDocumentListErrorMessage")]
        public string Validation { get; set; } = "PersonIdDocumentListValidation";
    }
}
