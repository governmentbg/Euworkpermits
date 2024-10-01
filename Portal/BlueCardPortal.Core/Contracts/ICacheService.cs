using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Core.Contracts
{
    public interface ICacheService
    {
        T GetValue<T>(string key) where T : class;
        void SetValue(string key, object data);
    }
}
