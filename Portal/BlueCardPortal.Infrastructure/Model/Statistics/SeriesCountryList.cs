using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Statistics
{
    public class SeriesCountryList
    {
        public int Page { get; set; }
        public List<string> Categories { get; set; } = new ();
        public List<SeriesCountryItem> Series { get; set; } = new();
    }
}
