using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Statistics
{
    public class SeriesCountryItem
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public int[] Data { get; set; } = new int[0];
    }
}
