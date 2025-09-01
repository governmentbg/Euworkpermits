using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Model.SelfDenial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts
{
    public partial interface IClient
    {
        string SerializeForSignature(ServiceApplication application);
        string SerializeForSignatureUpdate(object model);
    }
}
