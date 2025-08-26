using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Statistics
{
    public class SeriesVM
    {
        public List<SeriesItem> PermitData { get; set; } = new();
        public List<SeriesCountryList> CountryData { get; set; } = new();
        public SeriesCountryList CountryTop10Data { get; set; } = new();

        public SeriesCountryList YearData { get; set; } = new();
        public int CountryMaxPermit { get; set; }
        public int YearMaxPermit { get; set; }

     }
}
