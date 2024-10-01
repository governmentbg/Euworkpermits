using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    public class ForeignerSmallListVM
    {
        public List<ForeignerSmallVM> Items { get; set; } = new();
        [BCForeignerSmallList(ErrorMessage = "ForeignerSmallListErrorMessage")]
        public string Validation { get; set; } = "ContactListValidation";
    }
}
