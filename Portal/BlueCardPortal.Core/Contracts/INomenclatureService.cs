using BlueCardPortal.Infrastructure.Model.Common;
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
        Task SetViewBagApplication(dynamic ViewBag, string? applicationType);
        Task SetViewBagContact(dynamic ViewBag);
        Task SetViewBagPersonIdDocument(dynamic ViewBag);
        Task<List<SelectListItem>> GetBorderCrossingPoints(bool addChoice = true, string addChoiceText = "");
        Task<string> GetNomenclatureText(string nomenclatureType, string? value);
        Task SetViewBagApplicationFilter(dynamic ViewBag);
        bool ApplicantDisableOnPid();
        bool IsStartPermanent();
        List<SelectListItem> GetStatisticsYear();
        Task SaveFeedBack(FeedBackVM model);
        Task<List<SelectListItem>> GetNomenclatureCodeableDDL(string nomenclatureType, bool addChoice = true, string addChoiceText = "");
        Task<string?> UploadFileFormats();
        Task<int> UploadFileSize();
        Task<string[]> GetStatusesFor(string code);
    }
}
