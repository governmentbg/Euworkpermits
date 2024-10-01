using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    public class AddressListVM
    {
        public List<AddressVM> Items { get; set; } = new();
        [BCAddressList(ErrorMessage = "BCAddressListErrorMessage")]
        public string Validation { get; set; } = "AddressValidation";
    }
}
