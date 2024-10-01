using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Contracts;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Data.Models.Application;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Model;
using BlueCardPortal.Infrastructure.Model.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model.ApplicationList;
using BlueCardPortal.Infrastructure.Model.Complaint;
using BlueCardPortal.Infrastructure.Data.Models.Complaint;
using BlueCardPortal.Infrastructure.Data.Models.SelfDenial;
using BlueCardPortal.Infrastructure.Model.SelfDenial;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BlueCardPortal.Core.Services
{
    public class ApplicationService : BaseService, IApplicationService
    {
        private readonly IClient client;
        private readonly IStringLocalizer localizer;
        private readonly INomenclatureService nomenclatureService;
        public ApplicationService(IRepository _repo,
            ILogger<ApplicationService> _logger,
            IUserContext _userContext,
            IClient _client,
            IStringLocalizer _localizer,
            INomenclatureService _nomenclatureService)
        {
            repo = _repo;
            logger = _logger;
            userContext = _userContext;
            client = _client;
            localizer = _localizer;
            nomenclatureService = _nomenclatureService;
        }
        public T DeserializeDataObject<T>(string? data) where T : new()
        {
            var dateTimeConverter = new IsoDateTimeConverter() { DateTimeFormat = FormattingConstant.NormalDateFormat };
            if (data != null)
            {
                return JsonConvert.DeserializeObject<T>(data, new JsonConverter[] { dateTimeConverter });
            }
            else
            {
                return new T();
            }
        }
        public async Task<ApplicationVM> GetApplication(Guid? applicationId, bool forPreview = false, bool isForRemote = false)
        {
            var itemTypes = await repo.AllReadonly<ApplicationItemType>()
                                      .OrderBy(x => x.StepNum)
                                      .ToListAsync();
            List<ApplicationItem> items = null!;
            if (applicationId != null)
            {
                items = await repo.AllReadonly<ApplicationItem>()
                                  .Where(x => x.ApplicationId == applicationId)
                                  .ToListAsync();
            }
            var application = new ApplicationVM
            {
                ApplicationId = applicationId ?? Guid.NewGuid(),
            };
            var appllicationType = string.Empty;
            var number = 1;
            foreach (var itemType in itemTypes)
            {
                string? dataContent = items?.Where(x => x.ItemTypeId == itemType.Id).FirstOrDefault()?.DataContent;
                if (forPreview && !isForRemote && string.IsNullOrEmpty(dataContent))
                {
                    continue;
                }
                var model = itemType.Type switch
                {
                    // ToDo: Да се премахне при пълна функционалност
                    nameof(ApplicationTypeVM) => dataContent != null ? DeserializeDataObject<ApplicationTypeVM>(dataContent) : new ApplicationTypeVM() { ApplicationTypeCode = "2"},
                    nameof(ApplicantVM) => DeserializeDataObject<ApplicantVM>(dataContent),
                    nameof(ForeignerVM) => DeserializeDataObject<ForeignerVM>(dataContent),
                    nameof(ForeignerSmallListVM) => DeserializeDataObject<ForeignerSmallListVM>(dataContent),
                    nameof(EmployerVM) => DeserializeDataObject<EmployerVM>(dataContent),
                    nameof(EmploymentVM) => (object)DeserializeDataObject<EmploymentVM>(dataContent),
                    nameof(ApplicationInfoVM) => DeserializeDataObject<ApplicationInfoVM>(dataContent),
                    nameof(DocumentsVM) => DeserializeDataObject<DocumentsVM>(dataContent),
                    _ => null!
                };
                if (itemType.Type == nameof(ApplicationTypeVM))
                {
                    appllicationType = (model as ApplicationTypeVM)?.ApplicationTypeCode;
                }
                if (itemType.Type == nameof(ApplicantVM))
                {
                    var applicant = (model as ApplicantVM);
                    if (applicant != null)
                    {
                        if (userContext.PidType == "EGN")
                        {
                            applicant.Egn = userContext.Pid;
                        }
                        if (userContext.PidType == "LNCH")
                        {
                            applicant.Lnch = userContext.Pid;
                        }
                        applicant.Person.Employer.Identifier = userContext.Eik ?? string.Empty;
                    }
                }
                var stepItemVM = new StepItemVM
                {
                    Id = itemType.Id,
                    Number = itemType.StepNum == 3 ? itemType.StepNum : number,
                    CollapseName = forPreview ? $"previewAccordion{itemType.Id}" : $"step{itemType.Id}",
                    ViewName = forPreview ? itemType.PreviewNameView : itemType.ViewName,
                    HtmlPrefix = itemType.HtmlPrefix,
                    Label = forPreview ? itemType.PreviewName : itemType.Name,
                    Data = model,
                    IsDisabled = true
                };
                if (appllicationType != APPLICATION_TYPE.Temporary && itemType.Type == nameof(ForeignerSmallListVM))
                {
                    stepItemVM.IsHidden = true;
                }
                if (appllicationType == APPLICATION_TYPE.Temporary && 
                    (itemType.Type == nameof(ForeignerVM) || itemType.Type == nameof(EmploymentVM))
                   )
                {
                    stepItemVM.IsHidden = true;
                }
                if (!stepItemVM.IsHidden)
                {
                    number++;
                }
                application.ApplicationItems.Add(stepItemVM);
            }
            application.ApplicationItems.First().IsCurrent = true;
            application.ApplicationItems.First().IsDisabled = false;
            return application;
        }

        private async Task<ApplicationItem> SaveDataItem(Guid applicationId, string itemType, string dataContent)
        {
            var appItemType = await repo.All<ApplicationItemType>()
                                        .Where(x => x.Type == itemType)
                                        .FirstOrDefaultAsync();

            var appItem = await repo.All<ApplicationItem>()
                                    .Where(x => x.ApplicationId == applicationId &&
                                                x.ItemTypeId == appItemType!.Id)
                                    .FirstOrDefaultAsync();
            var application = await repo.All<Application>()
                                           .Where(x => x.Id == applicationId)
                                           .FirstOrDefaultAsync();
            if (application?.Status == ApplicationStatus.Send)
            {
                throw new Exception("ErrorAjax:Заявлението вече е изпратено");
            }
            if (appItem == null)
            {
                if (application == null)
                {
                    application = new Application
                    {
                        Id = applicationId,
                        DateWrt = DateTime.UtcNow,
                    };

                    await repo.AddAsync(application);
                }

                appItem = new ApplicationItem
                {
                    ItemTypeId = appItemType!.Id,
                    ApplicationId = applicationId,
                };

                // application.ApplicationItems.Add(appItem);
                await repo.AddAsync(appItem);
            }

            appItem.DateWrt = DateTime.UtcNow;
            appItem.DataContent = dataContent;
            application.DateWrt = DateTime.UtcNow;
            application.UserId = userContext.Id;
            await repo.SaveChangesAsync();
            return appItem;
        }
        public async Task SaveApplicant(Guid applicationId, ApplicantVM applicant)
        {
            await SaveDataItem(
                applicationId,
                nameof(ApplicantVM),
                JsonConvert.SerializeObject(applicant));
        }
        public async Task SaveApplicationType(Guid applicationId, ApplicationTypeVM applicationType)
        {
            await SaveDataItem(
                applicationId,
                nameof(ApplicationTypeVM),
                JsonConvert.SerializeObject(applicationType));
        }
        public async Task SaveForeigner(Guid applicationId, ForeignerVM foreigner)
        {
            await SaveDataItem(
                applicationId,
                nameof(ForeignerVM),
                JsonConvert.SerializeObject(foreigner));
        }
        public async Task SaveForeignerSmallList(Guid applicationId, ForeignerSmallListVM foreignerList)
        {
            await SaveDataItem(
                applicationId,
                nameof(ForeignerSmallListVM),
                JsonConvert.SerializeObject(foreignerList));
        }
        public async Task SaveEmployer(Guid applicationId, EmployerVM employer)
        {
            await SaveDataItem(
                applicationId,
                nameof(EmployerVM),
                JsonConvert.SerializeObject(employer));
        }

        public async Task SaveEmployment(Guid applicationId, EmploymentVM employment)
        {
            await SaveDataItem(
                 applicationId,
                 nameof(EmploymentVM),
                 JsonConvert.SerializeObject(employment));
        }
        public async Task SaveApplicationInfo(Guid applicationId, ApplicationInfoVM applicationInfo)
        {
            await SaveDataItem(
                applicationId,
                nameof(ApplicationInfoVM),
                JsonConvert.SerializeObject(applicationInfo));
        }
        public async Task SaveDocuments(Guid applicationId, DocumentsVM documents)
        {
            await SaveDataItem(
                applicationId,
                nameof(DocumentsVM),
                JsonConvert.SerializeObject(documents));
        }

        public async Task<DocumentResultVM> UploadFile(DocumentVM model)
        {
            var document = new ServiceDocument
            {
                //   Id = Guid.NewGuid().ToString(), //model.Id,
                //  CmisId = Guid.NewGuid().ToString(), //model.CmisId,
                //  DocumentIdentifier = "xxxx=dd",
                DocumentCategory = model.DocumentCategoryCode,
                IsMandatory = model.IsMandatory,
                IsOriginal = model.IsOriginal,
                FileName = model.FileName,
                ContentString = model.FileContent,
                UploadedByUser = string.IsNullOrEmpty(userContext.Name) ? "test" : userContext.Name,
                Title = string.IsNullOrEmpty(model.Title) ? model.DocumentType : model.Title,
                DocumentType = model.DocumentTypeCode,
                MimeType = model.MimeType,
                UploadedDate = DateTimeOffset.Now,
            };
            var body = new UploadDocument_input
            {
                ApplicationUid = model.ApplicationId.ToString(),
                Document = document,
                //ApplicationId = model.ApplicationId.ToString(),
            };

            var responce = await client.UploadDocumentAsync(body);
            var result = new DocumentResultVM
            {
                IsOk = responce.Status == "OK",
                Error = responce.Message,
                CmisId = responce.Document?.CmisId,
                PortalId = model.PortalId,
                Id = responce.Document?.Id,
                FileUrl = responce.Document?.Url,
                FileName = model?.FileName,
                HasMultipleFile = model.HasMultipleFile! ,
                HasTitle = model.HasTitle!,
                Title = model.Title,
                DocumentTypeCode = model.DocumentTypeCode,
            };
            model.FileUrl = result.FileUrl;
            if (!result.IsOk)
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            return result;
        }
        public async Task<DocumentResultVM> DownloadFile(string cmisId)
        {
            
            var body = new GetDocumentContent_input
            {
               CmisId = cmisId,
            };

            var responce = await client.GetDocumentContentAsync(body);
            var result = new DocumentResultVM
            {
                IsOk = responce.Status == "OK",
                Error = responce.Message,
                CmisId = cmisId,
                FileName = responce.FileName,
                Content = responce.ContentString,
                MimeType = responce.MimeType,
            };
            if (!result.IsOk)
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            return result;
        }
        public DocumentVM CreateOtherDocument(Guid applicationId)
        {
            return new DocumentVM
            {
                ApplicationId = applicationId,
                DocumentTypeCode = DocumentType.Other,
                DocumentType = localizer["DocumentTypeOther"],
                HasMultipleFile = true,
                HasTitle = true,
            };
        }
        public async Task<DocumentsVM> GetDocumentTypes(Guid applicationId, string permitType, string applicantType)
        {
            var application = await GetApplication(applicationId);
            var foreignerSmallList = application.GetForeignerSmallList();
            var applicationType = application.GetApplicationType();
            var documents = new DocumentsVM
            {
                PermitType = permitType,
                ApplicantType = applicantType
            };
            var types = await client.GetDocumentsNomenclatureAsync(new GetDocumentsNomenclature_input
            {
                ApplicantTypeValue = applicantType,
                PermitTypeValue = permitType
            });
            if (types.Status == "OK")
            {
                documents.Documents = types.Documents.Where(x => true || !(x.DocumentTypeValue == "2" || // TODO:
                                                        x.DocumentTypeValue == "4" ||
                                                        x.DocumentTypeValue == "8"))
                               .Select(x => new DocumentVM
                               {
                                   ApplicationId = applicationId,
                                   DocumentTypeCode = x.DocumentTypeValue,
                                   DocumentType = x.DocumentTypeName,
                                   DocumentCategoryCode = x.DocumentCategoryValue,
                                   DocumentCategory = x.DocumentCategoryName,
                                   IsMandatory = x.IsMandatory,
                               })
                              .ToList();
                if (false || foreignerSmallList != null && applicationType?.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
                {
                    var onForeignerDocuments = new List<DocumentVM>();
                    var documentTypes = types.Documents.Where(x => (x.DocumentTypeValue == "2" ||  // TODO:
                                                            x.DocumentTypeValue == "4" ||
                                                            x.DocumentTypeValue == "8"))
                                                        .ToList();
                    foreach (var foreigner in foreignerSmallList.Items)
                    {
                        onForeignerDocuments.AddRange(
                            documentTypes.Select(x => new DocumentVM
                            {
                                ApplicationId = applicationId,
                                DocumentTypeCode = x.DocumentTypeValue,
                                DocumentType = x.DocumentTypeName,
                                DocumentCategoryCode = x.DocumentCategoryValue,
                                DocumentCategory = x.DocumentCategoryName,
                                IsMandatory = x.IsMandatory,
                                ForeignerLabel = foreigner.NameCyrilic,
                                ForeignerSmallId = foreigner.Id
                            })
                            .ToList()
                        ); 
                    }
                    documents.Documents.AddRange(onForeignerDocuments);
                }
                // documents.Documents.Add(CreateOtherDocument(applicationId));
            }
            else
            {
                logger.LogError(types.Status + Environment.NewLine + types.Message);
            }
            return documents;
        }
        public async Task<List<SelectListItem>> GetDocumentTypesNomenclature(string permitType, string applicantType)
        {
            var result = new List<SelectListItem>();
            var types = await client.GetDocumentsNomenclatureAsync(new GetDocumentsNomenclature_input
            {
                ApplicantTypeValue = applicantType,
                PermitTypeValue = permitType
            });
            if (types.Status == "OK")
            {
                result = types.Documents
                               .Select(x => new SelectListItem
                               {
                                   Value = x.DocumentTypeValue,
                                   Text = x.DocumentTypeName,
                               })
                               .ToList();
            }
            else
            {
                logger.LogError(types.Status + Environment.NewLine + types.Message);
            }
            return result;
        }
        public ServiceEntity ForeignerToCoreApplicant(ApplicantVM model)
        {
            return new ServiceEntity
            {
                IdentifierNumber = model.Lnch,
                Name = model.Foreigner.NameCyrilic,
                TypeIdentifier = IDENTIFIER.Lnch,
                TypeIdentifierRepresentative = string.Empty,
                Fid = string.Empty,
                Address = ToCoreAddresses(model.Foreigner.Addresses),
                Employer = new ServiceEmployer(),
                ContactInfo = ToCoreContactInfoList(model.Foreigner.Contacts),
            };
        }
        public ApplicantVM ToVMApplicant( ServiceEntity applicant)
        {
            var applicantVM = new ApplicantVM();
            applicantVM.ApplicantType = applicant.Type;
            if (applicant.Type == ENTITY_TYPE.Foreigner) {
                applicantVM.Lnch = applicant.IdentifierNumber;
                applicantVM.Foreigner.NameCyrilic = applicant.Name;
                applicantVM.Foreigner.Addresses = ToVMAddresses(applicant.Address);
                applicantVM.Foreigner.Contacts = ToVMContactInfoList(applicant.ContactInfo);
            }
            else
            {
                applicantVM.Egn = applicant.IdentifierNumber;
                applicantVM.Person.Name = applicant.Name;
                applicantVM.Person.Address = ToVMAddress(applicant.Address.First());
                applicantVM.Person.Contacts = ToVMContactInfoList(applicant.ContactInfo);
                applicantVM.Person.ApplicantRole = string.IsNullOrEmpty(applicant.Employer?.Name) ? ApplicationRole.Person : ApplicationRole.Representative;
            }

            return applicantVM;
        }
        public ApplicationTypeVM ToVMApplicationType(ServiceApplication application)
        {
            return new ApplicationTypeVM
            {
                ApplicationTypeCode = application.ApplicationType,
                PermitType = application.ApplicationType,
            };
        }
        public ApplicationInfoVM ToVMApplicationInfo(ServiceApplication application)
        {
            return new ApplicationInfoVM
            {
                AdditionalInfo = application.AdditionalInfo,
            };
        }

        public ServiceEntity PersonToCoreApplicant(ApplicantVM model)
        {
            var addresses = new AddressListVM();
            addresses.Items.Add(model.Person.Address);
            // TODO: представител на фирма
            return new ServiceEntity
            {
                IdentifierNumber = model.Egn,
                Name = model.Person.Name,
                TypeIdentifier = IDENTIFIER.Egn,
                TypeIdentifierRepresentative = string.Empty,
                Fid = string.Empty,
                ContactInfo = ToCoreContactInfoList(model.Person.Contacts),
                Address = ToCoreAddresses(addresses),
                Employer = model.Person.ApplicantRole == ApplicationRole.Representative ? ToCoreEmployer(model.Person.Employer) : null
            };
        }
        public ServiceEntity ToCoreApplicant(ApplicationVM model)
        {
            var applicantVM = model.GetApplicant();
            if (applicantVM != null)
            {
                var applicant = applicantVM.ApplicantType == ENTITY_TYPE.Foreigner ?
                                                  ForeignerToCoreApplicant(applicantVM) :
                                                  PersonToCoreApplicant(applicantVM);
                applicant.Type = applicantVM.ApplicantType;
                return applicant;
            }
            return new ServiceEntity();
        }
        public List<ServiceAddress> ToCoreAddresses(AddressListVM addresses)
        {
            var result = new List<ServiceAddress>();
            foreach (var address in addresses.Items)
                result.Add(ToCoreAddress(address));
            return result;
        }
        public ServiceAddress ToCoreAddress(AddressVM address)
        {
            return new ServiceAddress
            {
                Kind = address.Kind,
                Country = COUNTRIES.Bulgaria,
                Region = address.Region,
                City = address.City,
                PostalCode = address.PostalCode.ToString(),
                Street = address.Street,
                StreetNo = address.StreetNo,
                Quarter = address.Quarter,
                BuildingNo = address.BuildingNo,
                Entrance = address.Entrance,
                Floor = address.Floor,
                Apartment = address.Apartment,
            };
        }

        public AddressListVM ToVMAddresses(ICollection<ServiceAddress> addresses)
        {
            var result = new AddressListVM();
            foreach (var address in addresses)
                result.Items.Add(ToVMAddress(address));
            return result;
        }
        public AddressVM ToVMAddress(ServiceAddress address)
        {
            var postcode = 0;
            int.TryParse(address.PostalCode, out postcode);
            return new AddressVM
            {
                Kind = address.Kind,
                Region = address.Region,
                City = address.City,
                PostalCode = postcode,
                Street = address.Street,
                StreetNo = address.StreetNo,
                Quarter = address.Quarter,
                BuildingNo = address.BuildingNo,
                Entrance = address.Entrance,
                Floor = address.Floor,
                Apartment = address.Apartment,
            };
        }

        public List<ServiceContactInfo> ToCoreContactInfoList(ContactListVM contacts)
        {
            var result = new List<ServiceContactInfo>();
            foreach (var contact in contacts.Items)
            {
                result.Add(new ServiceContactInfo
                {
                    Description = contact.Description,
                    IsPreferedContract = !string.IsNullOrEmpty(contact.IsPreferedContract),
                    Type = contact.Type,
                });
            }
            return result;
        }
        public ContactListVM ToVMContactInfoList(ICollection<ServiceContactInfo> contacts)
        {
            var result = new ContactListVM();
            foreach (var contact in contacts)
            {
                result.Items.Add(new ContactVM
                {
                    Description = contact.Description,
                    IsPreferedContract = contact.IsPreferedContract ? "Favorite" : string.Empty,
                    Type = contact.Type,
                });
            }
            return result;
        }
        public PersonIdDocumentVM ToVMIdDocument(ServiceIdDocument document)
        {
            return new PersonIdDocumentVM
            {
                ExpirationDate = document.ExpirationDate?.UtcDateTime,
                IssueDate = document.IssueDate?.UtcDateTime,
                IssuedBy = document.IssuedBy,
                Identifier = document.Identifier,
                Type = document.Type,
                Series = document.Series,
            };
        }

        public ServiceIdDocument ToCoreIdDocument(PersonIdDocumentVM document)
        {
            return new ServiceIdDocument
            {
                ExpirationDate = document.ExpirationDate,
                IssueDate = document.IssueDate,
                IssuedBy = document.IssuedBy,
                Identifier = document.Identifier,
                Type = document.Type,
                Series = document.Series,
            };
        }

        private DateTimeOffset? OldDateToNull(DateTime? date)
        {
            if (date == null)
                return null;
            return date > new DateTime(1800, 1, 1) ? date : null;
        }
        private DateTime OffsetToDateTime(DateTimeOffset? date)
        {
            return date?.UtcDateTime ?? DateTime.MinValue;
        }
        public ServiceForeigner ToCoreForeigner(ApplicationVM model)
        {
            var foreigner = model.GetForeigner();
            var employment = model.GetEmployment() ?? new EmploymentVM();
            if (foreigner != null)
            {
                return new ServiceForeigner
                {
                    Identifier = string.Empty,
                    Fid = foreigner.Identifier,
                    TypeIdentifier = foreigner.TypeIdentifier,
                    Name = foreigner.Name,
                    NameCyrilic = foreigner.NameCyrilic,
                    VisaExpirationDate = OldDateToNull(foreigner.VisaExpirationDate),
                    VisaSerialNumber = foreigner.VisaSerialNumber,
                    VisaType = foreigner.VisaType,
                    Nationality = foreigner.Nationality,
                    CityОfBirth = foreigner.CityОfBirth,
                    BirthDateTypeInput = BIRTH_DATE_TYPE_INPUT.YYYYMMDD,
                    BirthDate = foreigner.BirthDate,
                    Address = ToCoreAddresses(foreigner.Addresses),
                    ContactInfo = ToCoreContactInfoList(foreigner.Contacts),
                    Identifiers = foreigner.PersonIdDocuments.Items.Select(x => ToCoreIdDocument(x)).ToList(),
                    Gender = foreigner.Gender,
                    MaritalStatus = foreigner.MaritalStatus,
                    DurationOfEmploymentFrom = OldDateToNull(employment.DurationOfEmploymentFrom),
                    DurationOfEmploymentTo = OldDateToNull(employment.DurationOfEmploymentFrom.AddMonths(employment.DurationOfEmploymentMonth)),
                    Type = employment.Type,
                    Position = employment.Position,
                    // Qual = employment.QualificationVM,
                    EmploymentReason = employment.EmploymentReason,
                    EntryDate = foreigner.EntryDate == null ? null : OldDateToNull(foreigner.EntryDate.Value),
                    EntryPoint = foreigner.EntryPoint,
                    //EducationTypeVM 
                    //SpecialityVM
                    //SpecialityCodeVM 
                };
            }
            return new ServiceForeigner();
        }

        public ServiceForeigner ToCoreForeignerSmall(ForeignerSmallVM foreigner)
        {
            return new ServiceForeigner
            {
                Identifier = foreigner.PassportNumber,
                Name = foreigner.Name,
                NameCyrilic = foreigner.NameCyrilic,
                Nationality = foreigner.NationalityCode,
                BirthDateTypeInput = BIRTH_DATE_TYPE_INPUT.YYYYMMDD,
                BirthDate = foreigner.BirthDate,
                Position = foreigner.Position,
                DurationOfEmploymentFrom = foreigner.DurationOfEmploymentFrom,
                DurationOfEmploymentTo = foreigner.DurationOfEmploymentTo,
            };
        }
        public async Task<ForeignerSmallListVM> ToVMForeignerSmallList(ICollection<ServiceForeigner> foreigners)
        {
            var result = new ForeignerSmallListVM();
            if (foreigners != null)
            {
                foreach (var foreigner in foreigners)
                {
                    var foreignerVM = ToVMForeignerSmall(foreigner);
                    foreignerVM.Nationality = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.COUNTRIES, foreignerVM.NationalityCode);
                    result.Items.Add(foreignerVM);
                }
            }
            return result;
        }

        public ForeignerSmallVM ToVMForeignerSmall(ServiceForeigner foreigner)
        {
            return new ForeignerSmallVM
            {
                PassportNumber = foreigner.Identifier,
                Name = foreigner.Name,
                NameCyrilic = foreigner.NameCyrilic,
                NationalityCode = foreigner.Nationality,
                BirthDate = OffsetToDateTime(foreigner.BirthDate),
                Position = foreigner.Position,
                DurationOfEmploymentFrom = OffsetToDateTime(foreigner.DurationOfEmploymentFrom),
                DurationOfEmploymentTo = OffsetToDateTime(foreigner.DurationOfEmploymentTo),
            };
        }
        public ForeignerVM ToVMForeigner(ServiceForeigner foreigner)
        {
            return new ForeignerVM
            {
                Identifier = foreigner.Fid,
                TypeIdentifier = foreigner.TypeIdentifier,
                Name = foreigner.Name,
                NameCyrilic = foreigner.NameCyrilic,
                VisaExpirationDate = foreigner.VisaExpirationDate?.UtcDateTime,
                VisaSerialNumber = foreigner.VisaSerialNumber,
                VisaType = foreigner.VisaType,
                Nationality = foreigner.Nationality,
                CityОfBirth = foreigner.CityОfBirth,
                BirthDate = OffsetToDateTime(foreigner.BirthDate),
                Addresses = ToVMAddresses(foreigner.Address),
                Contacts = ToVMContactInfoList(foreigner.ContactInfo),
                PersonIdDocuments = new PersonIdDocumentListVM
                {
                    Items = foreigner.Identifiers.Select(x => ToVMIdDocument(x)).ToList()
                },
                Gender = foreigner.Gender,
                MaritalStatus = foreigner.MaritalStatus,
                EntryDate = foreigner.EntryDate?.UtcDateTime,
                EntryPoint = foreigner.EntryPoint,
            };
        }

        public EmploymentVM ToVMEmployment(ServiceApplication application)
        {
            var foreigner = application.Subject;
            var period = foreigner.DurationOfEmploymentTo - foreigner.DurationOfEmploymentFrom;
            var months = 0;
            if (period != null) {
                var days = period.Value.Days;
                months =days / 30;
            }
            return new EmploymentVM
            {
                DurationOfEmploymentFrom = OffsetToDateTime(foreigner.DurationOfEmploymentFrom),
                DurationOfEmploymentMonth = months,
                Type = foreigner.Type,
                Position = foreigner.Position,
                EmploymentReason = foreigner.EmploymentReason,
                SpecialityCode = application.NkpdPosition,
                Speciality = application.Specialty,
                //Type = application.ContractContinuation ? YESNO_TYPE.Yes : YESNO_TYPE.No,
                Qualification = application.Qualification,
            };
        }
        public ServiceEmployer ToCoreEmployer(EmployerVM employer)
        {
            if (employer != null)
            {
                return new ServiceEmployer
                {
                    Name = employer.Name,
                    IdentifierNumber = employer.Identifier,
                    EmployeeCount = employer.EmployeeCount,
                    ForeignEmployeeCount = employer.ForeignEmployeeCount,
                    Address = ToCoreAddresses(employer.GetAddresses()),
                    Type = employer.LegalForm,

                    ContactInfo = ToCoreContactInfoList(employer.Contacts),
                };
            }
            return new ServiceEmployer();
        }

        public EmployerVM ToVMEmployer(ServiceEmployer employer)
        {
            return new EmployerVM
            {
                Name = employer.Name,
                Identifier = employer.IdentifierNumber,
                EmployeeCount = employer.EmployeeCount,
                ForeignEmployeeCount = employer.ForeignEmployeeCount,
                Address = ToVMAddress(employer.Address.Where(x => x.Kind == ADDRESSE_TYPE.Head).FirstOrDefault() ?? new ServiceAddress()),
                ContactAddress = ToVMAddress(employer.Address.Where(x => x.Kind == ADDRESSE_TYPE.Correspondence).FirstOrDefault() ?? new ServiceAddress()),
                LegalForm = employer.Type,
                Contacts = ToVMContactInfoList(employer.ContactInfo),
            };
        }

        public List<ServiceDocument> ToCoreDocuments(DocumentsVM documents)
        {
            var result = new List<ServiceDocument>();
            foreach (var document in documents.Documents)
            {
                var documentTitle = document.Title;
                if (string.IsNullOrEmpty(documentTitle))
                {
                    documentTitle = document.DocumentType;
                }
                result.Add(new ServiceDocument
                {
                    //   Id = Guid.NewGuid().ToString(), //model.Id,
                    CmisId = document.CmisId,
                    DocumentCategory = document.DocumentCategoryCode,
                    IsMandatory = document.IsMandatory,
                    IsOriginal = document.IsOriginal,
                    FileName = document.FileName,
                    Title = document.Title,
                    DocumentType = document.DocumentTypeCode,
                    MimeType = document.MimeType,
                    Url = document.FileUrl,
                    UploadedByUser = userContext.Name,
                    UploadedDate = DateTimeOffset.Now,
                });
            }
            return result;
        }

        public async Task<DocumentsVM> ToVMDocuments(ServiceApplication application)
        {
            var result = new DocumentsVM { 
                ApplicantType = application.Applicant.Type,
                PermitType = application.PermitType,    
            };
            var documentTypes = await GetDocumentTypesNomenclature(result.PermitType, result.ApplicantType);
            foreach (var document in application.Documents)
            {
                var documentVM = new DocumentVM
                {
                    //   Id = Guid.NewGuid().ToString(), //model.Id,
                    CmisId = document.CmisId,
                    DocumentCategoryCode = document.DocumentCategory,
                    IsMandatory = document.IsMandatory,
                    IsOriginal = document.IsOriginal,
                    FileName = document.FileName,
                    DocumentTypeCode = document.DocumentType,
                    MimeType = document.MimeType,
                    FileUrl = document.Url,
                };
                documentVM.DocumentType = documentTypes.Where(x => x.Value == documentVM.DocumentTypeCode).Select(x => x.Text).FirstOrDefault() ?? string.Empty;
                result.Documents.Add(documentVM);
            }
            return result;
        }


        public async Task<CreateApplication_input> ToCoreApplication(ApplicationVM model)
        {
            var applicationType = model.GetApplicationType();
            var applicationInfo = model.GetApplicationInfo();
            var application = new ServiceApplication
            {
                ApplicationUid = model.ApplicationId.ToString(),
                ApplicationType = applicationType?.ApplicationTypeCode,
                PermitType = applicationType?.PermitType,
                Applicant = ToCoreApplicant(model),
                Employer = ToCoreEmployer(model.GetEmployer()!),
                Documents = ToCoreDocuments(model.GetDocuments()!),
                AdditionalInfo = applicationInfo?.AdditionalInfo,
            };
            if (applicationType?.ApplicationTypeCode == APPLICATION_TYPE.Permanent)
            {
                var employment = model.GetEmployment();
                application.ContractContinuation = employment?.Type == YESNO_TYPE.Yes;
                application.Position = employment?.Position;
                application.Qualification = employment?.Qualification;
                application.Specialty = employment?.Speciality;
                application.EducationType = employment?.EducationType;
                application.NkpdPosition = employment?.SpecialityCode;
                application.EmploymentAddress = application.Employer.Address.FirstOrDefault();
                application.Subject = ToCoreForeigner(model);
                application.EmploymentAddressMatch = employment?.AddressIsSame == YESNO_TYPE.Yes;
            }
            if (applicationType?.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                var foreignerSmallList = model.GetForeignerSmallList();
                application.PermitType = PERMIT_TYPE.Temporary;
                application.SubjectLists = foreignerSmallList.Items.Select(x => ToCoreForeignerSmall(x)).ToList();
                application.EmploymentAddressMatch = true; 
            }
            
            return new CreateApplication_input
            {
                Application = application,
            };
        }
        public ServiceComplaint ToCoreComplaint(ComplaintVM model)
        {
            var applicant = model.Applicant.ApplicantType == ENTITY_TYPE.Foreigner ?
                                                  ForeignerToCoreApplicant(model.Applicant) :
                                                  PersonToCoreApplicant(model.Applicant);

             return new ServiceComplaint
            {
                Complainant = applicant,
                Comment = model.ComplaintInfo,
                ApplicationId = model.ApplicationNumber,
            };
        }
        public async Task<(string, string)> SendApplication(Guid applicationId)
        {
            var applicationVM = await GetApplication(applicationId);
            var application_input = await ToCoreApplication(applicationVM);
            var result = await client.CreateApplicationAsync(application_input);
            if (result.Status == "OK")
            {
                var application = await repo.All<Application>().Where(x => x.Id == applicationId).FirstAsync();
                application.Status = ApplicationStatus.Send;
                application.ApplyNumber = result.ApplicationId;
                await repo.SaveChangesAsync();
            }
            else
            {
                logger.LogError(result.Status + Environment.NewLine + result.Message);
            }
            return (result.Status, result.Status == "OK" ? string.Empty : result.Message);
        }

        public async Task SetStatusDraft(Guid applicationId)
        {
            var application = await repo.All<Application>().Where(x => x.Id == applicationId).FirstOrDefaultAsync();
            if (application != null)
            {
                application.Status = ApplicationStatus.Draft;
                await repo.SaveChangesAsync();
            }
        }
        public async Task<ApplicationVM> GetApplicationRemote(string applicationId)
        {
            var responce = await client.GetApplicationAsync(new GetApplication_input { ApplicationId = applicationId });
            if (responce.Status != "OK")
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
                return null!;
            }
            var application = await GetApplication(Guid.NewGuid(), true, true);
            var applicationType = ToVMApplicationType(responce.Application);
            application.SetData(applicationType);
            application.SetData(ToVMApplicant(responce.Application.Applicant));
            application.SetData(ToVMForeigner(responce.Application.Subject));
            application.SetData(ToVMEmployer(responce.Application.Employer));
            application.SetData(ToVMEmployment(responce.Application));
            application.SetData(ToVMApplicationInfo(responce.Application));
            application.SetData(await ToVMDocuments(responce.Application));
            application.SetData(await ToVMForeignerSmallList(responce.Application.SubjectLists)); 
            if (applicationType.ApplicationTypeCode == APPLICATION_TYPE.Permanent)
            {
                application.ApplicationItems = application.ApplicationItems
                                                          .Where(x => !(x.Data is ForeignerSmallListVM))
                                                          .ToList();
            }
            if (applicationType.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                application.ApplicationItems = application.ApplicationItems
                                                          .Where(x => !(x.Data is ForeignerVM) && !(x.Data is EmploymentVM))
                                                          .ToList();
            }
            return application;
        }
        public async Task<List<ApplicationListItemVM>> GetApplicationList()
        {
            var responce = await client.ListApplicationsAsync(new ListApplications_input { IdentifierNumber =  userContext.Pid, RecordsPerPage = 100000, Bulstat = userContext.Eik });
            var result = new List<ApplicationListItemVM>();
            if (responce.Status == "OK")
            {
                foreach (var item in responce.Applications)
                {
                    var sNameCyrilic = item.SubjectNameCyrilic.ToList();
                    var sBirthDate = item.SubjectBirthDate?.ToList();
                    var sNationality = item.SubjectNationality?.ToList();
                    for (int i = 0; i < sNameCyrilic.Count; i++)
                    {
                        var applicationListItem = new ApplicationListItemVM
                        {
                            ApplicationId = item.ApplicationId, 
                            ApplicationNumber = item.ApplicationId,
                            ForeignerName = sNameCyrilic[i],
                            ForeignerLNCH = item.SubjectFid,
                            ForeignerBirthDate = sBirthDate == null ?
                                                 string.Empty :
                                                 sBirthDate[i]?.ToLocalTime().ToString(FormattingConstant.NormalDateFormat),
                            ForeignerNationality = sNationality == null ? string.Empty : sNationality[i],
                            PermitTypeCode = item.PermitType,
                            StatusCode = item.Status,
                            EntryDate = item.RegistrationDate?.LocalDateTime
                        };
                        applicationListItem.ForComplaint = EXTERNAL_STATUS.ForComplaint.Contains(applicationListItem.Status);
                        applicationListItem.ForSelfDenial = !INTERNAL_STATUS.NoSelfDenial.Contains(item.InternalStatus);
                        applicationListItem.ForUpdate = applicationListItem.Status == EXTERNAL_STATUS.WaitDocuments;
                        applicationListItem.PermitType = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.PERMIT_TYPE, applicationListItem.PermitTypeCode);
                        applicationListItem.Status = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.EXTERNAL_STATUS, applicationListItem.StatusCode);
                        result.Add(applicationListItem);
                    }
                }
            }
            else
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            return result;
        }

        public async Task<GridResponseModel> GetComplaintList(int? inPage, long? inPageSize)
        {
            var responce = await client.ListComplaintsAsync(new ListComplaints_input { IdentifierNumber = userContext.Pid });
            var result = new List<ComplaintListItemVM>();
            if (responce.Status == "OK")
            {
                foreach (var item in responce.Complaints)
                {
                    var complaintListItem = new ComplaintListItemVM
                    {
                        ComplaintNumber = item.ComplaintId,
                        ApplicationNumber = item.ApplicationId,
                        ComplaintName = item.ComplainantName,
                        Status = item.Status,
                        StatusDate = item.StatusDate?.LocalDateTime,
                        ComplaintDate = item.StatusDate?.LocalDateTime
                    };
                    complaintListItem.Status = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.EXTERNAL_STATUS, complaintListItem.Status);
                    result.Add(complaintListItem);
                }
            }
            else
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            var query = result.AsQueryable();
            return CalcGridResponseModel(query, inPage, inPageSize);
        }
    

        public async Task<EmployerVM> GetEmployer(string uic)
        {
            var responce = await client.GetEmployerAsync(new GetEmployer_input { IdentifierNumber = uic });
            if (responce.Status != "OK")
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            return ToVMEmployer(responce.Employer);
        }

        public async Task<GridResponseModel> FilterApplicationList(string filterJson, List<ApplicationListItemVM> data, int? inPage, long? inPageSize)
        {
            var filter = DeserializeDataObject<ApplicationFilterVM>(filterJson);
            var country = string.IsNullOrEmpty(filter.Country) ?
                          string.Empty :
                          await nomenclatureService.GetNomenclatureText(NomenclatureTypes.COUNTRIES, filter.Country);
            var query = data
                       .Where(x => string.IsNullOrEmpty(filter.ForeignerName) || x.ForeignerName?.ToUpper().Contains(filter.ForeignerName.ToUpper()) == true)
                       .Where(x => string.IsNullOrEmpty(filter.LNCH) || x.ForeignerName.Contains(filter.LNCH))
                       .Where(x => string.IsNullOrEmpty(filter.ApplicationNumber) || x.ApplicationNumber == filter.ApplicationNumber)
                       .Where(x => string.IsNullOrEmpty(filter.PermitType) || x.PermitTypeCode == filter.PermitType)
                       .Where(x => string.IsNullOrEmpty(filter.Status) || x.StatusCode == filter.Status)
                       .Where(x => string.IsNullOrEmpty(country) || x.ForeignerNationality == country)
                       .Where(x => filter.FromDate == null || filter.FromDate <= x.EntryDate)
                       .Where(x => filter.ToDate == null || x.EntryDate <= filter.ToDate)
                       .Where(x => filter.BirthDate == null || x.ForeignerBirthDate == filter.BirthDate?.ToString(FormattingConstant.NormalDateFormat))
                       .AsQueryable();
            return CalcGridResponseModel<ApplicationListItemVM>(query, inPage, inPageSize);
        }
        public async Task<GridResponseModel> GetApplicationListDraft(int? inPage, long? inPageSize)
        {
            var applicationItemTypes = await repo.AllReadonly<ApplicationItemType>().ToListAsync();
            var applications = await repo.AllReadonly<Application>()
                                         .Where(x => x.UserId == userContext.Id &&
                                                     x.Status == ApplicationStatus.Draft)
                                         .Include(x => x.ApplicationItems)
                                         .ToListAsync();
            var applicationList = new List<ApplicationListItemVM>();
            foreach (var application in applications)
            {
                var applicationListItem = new ApplicationListItemVM
                {
                    ApplicationId = application.Id.ToString(),
                };
                var foreignerType = applicationItemTypes.Where(x => x.Type == nameof(ForeignerVM)).FirstOrDefault();
                var foreignerItem = application.ApplicationItems.Where(x => x.ItemTypeId == foreignerType!.Id).FirstOrDefault();
                if (foreignerItem != null)
                {
                    var foreigner = DeserializeDataObject<ForeignerVM>(foreignerItem.DataContent);
                    applicationListItem.ForeignerName = foreigner.Name;
                }
                var appTypeType = applicationItemTypes.Where(x => x.Type == nameof(ApplicationTypeVM)).FirstOrDefault();
                var appTypeItem = application.ApplicationItems.Where(x => x.ItemTypeId == appTypeType!.Id).FirstOrDefault();
                if (appTypeItem != null)
                {
                    var appType = DeserializeDataObject<ApplicationTypeVM>(appTypeItem.DataContent);
                    applicationListItem.PermitType = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.PERMIT_TYPE, appType.PermitType);
                }
                applicationList.Add(applicationListItem);
            }
            var query = applicationList.AsQueryable();
            return CalcGridResponseModel<ApplicationListItemVM>(query, inPage, inPageSize);
        }
        public async Task<(bool, string)> SaveComplaint(ComplaintVM complaintVm)
        {
            var complaint = await repo.All<Complaint>()
                                      .Where(x => x.Id == complaintVm.Id)
                                      .FirstOrDefaultAsync();
            if (complaint == null)
            {
                complaint = new Complaint
                {
                    Id = complaintVm.Id,
                };
                await repo.AddAsync(complaint);
            }
            complaint.ApplyNumberFrom = complaintVm.ApplicationNumber;
            complaint.DateWrt = DateTime.UtcNow;
            complaint.DataContent = JsonConvert.SerializeObject(complaintVm);
            complaint.UserId = userContext.Id;
            await repo.SaveChangesAsync();
            var body = new CreateComplaint_input { Complaint = ToCoreComplaint(complaintVm) };
            var responce = await client.CreateComplaintAsync(body);
            if (responce.Status != "OK")
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
                return (false, responce.Message);

            }
            complaint.ComplaintNumber = responce.ComplaintId;
            await repo.SaveChangesAsync();
            return (true, $"Подадена е жалба {responce.ComplaintId}");
        }
        public async Task<(bool, string)> SaveApplicationUpdate(ApplicationUpdateVM applicationUpdateVm)
        {
            var applicationUpdate = await repo.All<ApplicationUpdate>()
                                      .Where(x => x.Id == applicationUpdateVm.Id)
                                      .FirstOrDefaultAsync();
            if (applicationUpdate == null)
            {
                applicationUpdate = new ApplicationUpdate
                {
                    Id = applicationUpdateVm.Id,
                };
                await repo.AddAsync(applicationUpdate);
            }
            applicationUpdate.ApplyNumberFrom = applicationUpdateVm.ApplicationNumber;
            applicationUpdate.DateWrt = DateTime.UtcNow;
            applicationUpdate.DataContent = JsonConvert.SerializeObject(applicationUpdateVm);
            applicationUpdate.UserId = userContext.Id;
            await repo.SaveChangesAsync();
            var body = new UpdateApplication_input
            {
                ApplicationId = applicationUpdateVm.ApplicationNumber,
                Documents = ToCoreDocuments(applicationUpdateVm.Documents),
            };
            var responce = await client.UpdateApplicationAsync(body);
            if (responce.Status != "OK")
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
                return (false, responce.Message);

            }
            applicationUpdate.Status = 99;
            await repo.SaveChangesAsync();
            return (true, $"Подадена е промяна по заявление {applicationUpdateVm.ApplicationNumber}");
        }
        
        public async Task<(bool, string)> SaveSelfDenial(SelfDenialVM rejectionVm)
        {
            var rejection = await repo.All<SelfDenial>()
                                      .Where(x => x.Id == rejectionVm.Id)
                                      .FirstOrDefaultAsync();
            if (rejection == null)
            {
                rejection = new SelfDenial
                {
                    Id = rejectionVm.Id,
                };
                await repo.AddAsync(rejection);
            }
            rejection.ApplyNumberFrom = rejectionVm.ApplicationNumber;
            rejection.DateWrt = DateTime.UtcNow;
            rejection.DataContent = JsonConvert.SerializeObject(rejectionVm);
            rejection.UserId = userContext.Id;
            await repo.SaveChangesAsync();

            var body = new UpdateApplication_input { 
                InternalStatus = INTERNAL_STATUS.SelfDenial, 
                ApplicationId = rejectionVm.ApplicationNumber 
            };
            var responce = await client.UpdateApplicationAsync(body);
            if (responce.Status != "OK")
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
                return (false, responce.Message);
            }
            rejection.Status = int.Parse(INTERNAL_STATUS.SelfDenial);
            await repo.SaveChangesAsync();
            return (true, $"Подаден е самоотказ за {rejection.ApplyNumberFrom}");
        }
    }
}
