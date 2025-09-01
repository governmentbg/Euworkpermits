using BlueCardPortal.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    public class ContactListVM 
    {
        public List<ContactVM> Items { get; set; } = new();
        [BCContactList(ErrorMessage = "ContactListErrorMessage", ErrorMessagePrefered = "ContactListPreferedErrorMessage")]
        public string Validation {  get; set; } = "ContactListValidation";

        public void AddNewIfEmpty()
        {
            if (Items.Count == 0)
                Items.Add(new ContactVM());
        }
    }
}
