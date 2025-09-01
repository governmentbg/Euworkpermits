using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Statistics
{
    public class DashboardFilter
    {
        [Display(Name = "DashboardYear")]
        public string Year { get; set; } = DateTime.Today.Year.ToString();

        [Display(Name = "DashboardYear")]
        public string YearCountry { get; set; } = DateTime.Today.Year.ToString();
        [Display(Name = "DashboardYear")]
        public string YearCountryTop10 { get; set; } = DateTime.Today.Year.ToString();
        [Display(Name = "DashboardYearFrom")]
        public string recapFromYear { get; set; } = string.Empty;
        [Display(Name = "DashboardYearTo")]
        public string recapToYear { get; set; } = DateTime.Today.Year.ToString();
        [Display(Name = "Country")]
        public string[]? Countries { get; set; }
    }
}
