using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Contracts;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueCardPortal.Core.Services
{
    public class NomenclatureService : BaseService, INomenclatureService
    {
        private readonly IClient client;
        private readonly IStringLocalizer localizer;
        private readonly ICacheService cacheService;
        public NomenclatureService(IRepository _repo,
            ILogger<NomenclatureService> _logger,
            IUserContext _userContext,
            IClient _client,
            IStringLocalizer _localizer,
            ICacheService _cacheService
            )
        {
            repo = _repo;
            logger = _logger;
            userContext = _userContext;
            client = _client;
            localizer = _localizer;
            cacheService = _cacheService;
        }
        public void AddChoice(List<SelectListItem> ddl, string addChoiceText)
        {
            ddl.Insert(0,
                new SelectListItem
                {
                    Value = null,
                    Text = addChoiceText,
                    Disabled = true,
                    Selected = true,
                });
        }
        public async Task<List<SelectListItem>> GetNomenclatureDDL(string nomenclatureType, bool addChoice = true, string addChoiceText = "")
        {
            var nomenclatures = cacheService.GetValue<GetNomenclatures_output>(nomenclatureType);
            if (nomenclatures == null)
            {
                nomenclatures = await client.GetNomenclaturesAsync(new GetNomenclatures_input()
                {
                    ReferenceDataName = nomenclatureType,
                });
                if (nomenclatures.Status != "OK")
                {
                    logger.LogError(nomenclatures.Status + Environment.NewLine + nomenclatures.Message);
                }
                cacheService.SetValue(nomenclatureType, nomenclatures);
            }

            var ddl = nomenclatures!.ReferenceData
                        .Select(x => new SelectListItem
                        {
                            Value = x.Code,
                            Text = x.DisplayName,
                        })
                        .ToList();
            if (addChoice)
            {
                AddChoice(ddl, string.IsNullOrEmpty(addChoiceText) ? "Изберете" : addChoiceText);
            }
            return ddl;
        }
        public async Task SetViewBagApplication(dynamic ViewBag)
        {
            ViewBag.ApplicationType_ddl = await GetNomenclatureDDL(NomenclatureTypes.APPLICATION_TYPE, false);
            ViewBag.PermitType_ddl = (await GetNomenclatureDDL(NomenclatureTypes.PERMIT_TYPE, false))
                                     .Where(x => x.Value != PERMIT_TYPE.Temporary)
                                     .ToList();
            ViewBag.ApplicantType_ddl = (await GetNomenclatureDDL(NomenclatureTypes.ENTITY_TYPE, false))
                                        .OrderByDescending(x => x.Text)
                                        .ToList();

            ViewBag.ApplicantRole_ddl = GetApplicantRoleDDL();


            ViewBag.ForeignerTypeIdentifier_ddl = await GetNomenclatureDDL(NomenclatureTypes.FOREIGNER_TYPE_IDENTIFIER, false);
            ViewBag.MaritalStatus_ddl = await GetNomenclatureDDL(NomenclatureTypes.MARITAL_STATUS, true);
            ViewBag.Gender_ddl = await GetNomenclatureDDL(NomenclatureTypes.GENDER, true);
            ViewBag.VisaType_ddl = await GetNomenclatureDDL(NomenclatureTypes.VISA_TYPE, true);
            ViewBag.EmploymentType_ddl = GetYesNoDDL();
            ViewBag.SpecialityCode_ddl = await GetNomenclatureDDL(NomenclatureTypes.NKPD_CODE, true);
            ViewBag.EducationType_ddl = await GetNomenclatureDDL(NomenclatureTypes.EDUCATION, true);
            ViewBag.Countries_ddl = await GetNomenclatureDDL(NomenclatureTypes.COUNTRIES, true);
            ViewBag.LegalForm_ddl = await GetNomenclatureDDL(NomenclatureTypes.LEGAL_FORM_TYPE, true);
            ViewBag.EntryPoint_ddl = await GetBorderCrossingPoints();
            ViewBag.AddressIsSame_ddl = GetYesNoDDL();
            await SetViewBagAddress(ViewBag);
            await SetViewBagContact(ViewBag);
            await SetViewBagPersonIdDocument(ViewBag);
        }
        public async Task SetViewBagAddress(dynamic ViewBag)
        {
            ViewBag.Region_ddl = await GetNomenclatureDDL(NomenclatureTypes.REGION, true);
            ViewBag.City_ddl = await GetRegionCities(string.Empty);
        }
        public async Task SetViewBagContact(dynamic ViewBag)
        {
            ViewBag.ContactType_ddl = await GetNomenclatureDDL(NomenclatureTypes.CONTACT_INFO_TYPE, true);
            ViewBag.Favorite_ddl = GetFavoriteDDL();
        }
        public async Task SetViewBagApplicationFilter(dynamic ViewBag)
        {
            ViewBag.PermitType_ddl = await GetNomenclatureDDL(NomenclatureTypes.PERMIT_TYPE, true);
            ViewBag.Status_ddl = await GetNomenclatureDDL(NomenclatureTypes.EXTERNAL_STATUS, true);
            ViewBag.Countries_ddl = await GetNomenclatureDDL(NomenclatureTypes.COUNTRIES, true);
        }

        public async Task SetViewBagPersonIdDocument(dynamic ViewBag)
        {
            ViewBag.PersonIdDocument_ddl = await GetNomenclatureDDL(NomenclatureTypes.TYPE_IDENTIFICATION_DOCUMENT, true);
        }

        public List<SelectListItem> GetFavoriteDDL()
        {
            return new List<SelectListItem> {
                new SelectListItem {
                    Value = "Favorite",
                    Text = localizer["Favorite"]
                }
            };
        }


        public List<SelectListItem> GetApplicantRoleDDL()
        {
            return new List<SelectListItem> {
                new SelectListItem {
                    Value = "1",
                    Text = localizer["ApplicantRole1"]
                },
                new SelectListItem {
                    Value = "2",
                    Text = localizer["ApplicantRole2"]
                }
            };
        }
        public List<SelectListItem> GetYesNoDDL()
        {
            return new List<SelectListItem> {
                new SelectListItem {
                    Value = YESNO_TYPE.Yes,
                    Text = localizer[YESNO_TYPE.Yes]
                },
                new SelectListItem {
                    Value = YESNO_TYPE.No,
                    Text = localizer[YESNO_TYPE.No]
                }
            };
        }
        public async Task<List<SelectListItem>> GetRegionCities(string region, bool addChoice = true, string addChoiceText = "")
        {
            var ddl = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(region))
            {
                var cities = cacheService.GetValue<GetRegionCities_output>($"GetRegionCities{region}");
                if (cities == null)
                {
                    cities = await client.GetRegionCitiesAsync(new GetRegionCities_input { Region = region });
                    if (cities.Status != "OK")
                    {
                        logger.LogError(cities.Status + Environment.NewLine + cities.Message);
                    }

                    cacheService.SetValue($"GetRegionCities{region}", cities);
                }
                ddl = cities.RegionCities
                               .Select(x => new SelectListItem
                               {
                                   Value = x,
                                   Text = x,
                               })
                               .ToList();
            }
            if (addChoice)
            {
                AddChoice(ddl, string.IsNullOrEmpty(addChoiceText) ? "Изберете" : addChoiceText);
            }
            return ddl;
        }

        public async Task<List<SelectListItem>> GetBorderCrossingPoints(bool addChoice = true, string addChoiceText = "")
        {
            var borderCrossingPoints = cacheService.GetValue<GetBorderCrossingPoints_output>("GetBorderCrossingPoints");
            if (borderCrossingPoints == null)
            {
                borderCrossingPoints = await client.GetBorderCrossingPointsAsync(new GetBorderCrossingPoints_input());
                if (borderCrossingPoints.Status != "OK")
                {
                    logger.LogError(borderCrossingPoints.Status + Environment.NewLine + borderCrossingPoints.Message);
                }

                cacheService.SetValue("GetBorderCrossingPoints", borderCrossingPoints);
            }
            var ddl = borderCrossingPoints.BorderCrossingPoints
                           .Select(x => new SelectListItem
                           {
                               Value = x.Name,
                               Text = x.Name,
                           })
                           .ToList();
            if (addChoice)
            {
                AddChoice(ddl, string.IsNullOrEmpty(addChoiceText) ? "Изберете" : addChoiceText);
            }
            return ddl;
        }
        public async Task<string> GetNomenclatureText(string nomenclatureType, string? value)
        {
            var ddl = nomenclatureType switch
            {
                nameof(YESNO_TYPE) => GetYesNoDDL(),
                _ => await GetNomenclatureDDL(nomenclatureType)
            };

            return ddl.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault() ?? string.Empty;
        }
    }
}
