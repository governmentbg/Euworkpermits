using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Statistics
{
    public class SeriesItem
    {
        public string? Name { get; set; }
        public decimal Y { get; set; }
        public string? Code { get; set; }
        public string? Color { get; set; }
    }
}
