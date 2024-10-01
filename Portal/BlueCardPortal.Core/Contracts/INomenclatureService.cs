using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Core.Contracts
{
    public interface INomenclatureService : IBaseService
    {
        Task<List<SelectListItem>> GetRegionCities(string region, bool addChoice = true, string addChoiceText = "");
        Task<List<SelectListItem>> GetNomenclatureDDL(string nomenclatureType, bool addChoice = true, string addChoiceText = "");
        Task SetViewBagAddress(dynamic ViewBag);
        Task SetViewBagApplication(dynamic ViewBag);
        Task SetViewBagContact(dynamic ViewBag);
        Task SetViewBagPersonIdDocument(dynamic ViewBag);
        Task<List<SelectListItem>> GetBorderCrossingPoints(bool addChoice = true, string addChoiceText = "");
        Task<string> GetNomenclatureText(string nomenclatureType, string? value);
        Task SetViewBagApplicationFilter(dynamic ViewBag);
    }
}
