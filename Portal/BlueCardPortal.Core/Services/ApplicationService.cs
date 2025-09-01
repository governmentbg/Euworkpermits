using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Contracts;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Data.Models.Application;
using BlueCardPortal.Infrastructure.Data.Models.Complaint;
using BlueCardPortal.Infrastructure.Data.Models.SelfDenial;
using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using BlueCardPortal.Infrastructure.Model;
using BlueCardPortal.Infrastructure.Model.Application;
using BlueCardPortal.Infrastructure.Model.ApplicationList;
using BlueCardPortal.Infrastructure.Model.Complaint;
using BlueCardPortal.Infrastructure.Model.SelfDenial;
using BlueCardPortal.Infrastructure.Model.Statistics;
using IO.SignTools.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using static NodaTime.TimeZones.TzdbZone1970Location;



namespace BlueCardPortal.Core.Services
{
    public class ApplicationService : BaseService, IApplicationService
    {
        private readonly IClient client;
        private readonly IStringLocalizer localizer;
        private readonly INomenclatureService nomenclatureService;
        private readonly ISignerService signerService;
        private readonly IConfiguration configuration;
        public ApplicationService(IRepository _repo,
            ILogger<ApplicationService> _logger,
            IUserContext _userContext,
            IClient _client,
            IStringLocalizer _localizer,
            INomenclatureService _nomenclatureService,
            ISignerService _signerService,
            IConfiguration _configuration)
        {
            repo = _repo;
            logger = _logger;
            userContext = _userContext;
            client = _client;
            localizer = _localizer;
            nomenclatureService = _nomenclatureService;
            signerService = _signerService;
            configuration = _configuration;
        }
        public T DeserializeDataObject<T>(string? data) where T : new()
        {
            var dateTimeConverter = new IsoDateTimeConverter() { DateTimeFormat = FormattingConstant.NormalDateFormat };
            if (data != null)
            {
                return JsonConvert.DeserializeObject<T>(data, [dateTimeConverter])!;
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
            var app = await repo.AllReadonly<Application>()
                                .Where(x => x.Id == applicationId)
                                .FirstOrDefaultAsync();
            var application = new ApplicationVM
            {
                ApplicationId = applicationId ?? Guid.NewGuid(),
                ApplyNumber = app?.ApplyNumber,
                Status = app?.Status ?? 0
            };
            var appllicationType = string.Empty;
            var permitType = string.Empty;
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
                    nameof(ApplicationTypeVM) => DeserializeDataObject<ApplicationTypeVM>(dataContent),
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
                    var appType = (model as ApplicationTypeVM);
                    if (appType != null && string.IsNullOrEmpty(appType.ApplicationTypeCode) && !nomenclatureService.IsStartPermanent())
                    {
                        appType.ApplicationTypeCode = APPLICATION_TYPE.Temporary;
                    }
                    appllicationType = appType?.ApplicationTypeCode;
                    permitType = appType?.PermitType;
                }
                if (itemType.Type == nameof(ApplicantVM))
                {
                    var applicant = (model as ApplicantVM);
                    if (applicant != null && string.IsNullOrEmpty(applicant.UicType))
                    {
                        InitNewApplicant(applicant);
                    }
                }
                if (itemType.Type == nameof(ForeignerVM))
                {
                    var foreigner = (model as ForeignerVM);
                    foreigner?.Contacts.AddNewIfEmpty();
                }
                if (itemType.Type == nameof(EmployerVM))
                {
                    var employer = (model as EmployerVM);
                    employer?.Contacts.AddNewIfEmpty();
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
                    IsDisabled = true,
                    IsTemporary = appllicationType == APPLICATION_TYPE.Temporary,
                    PermitType = permitType,
                };
                if (appllicationType != APPLICATION_TYPE.Temporary && itemType.Type == nameof(ForeignerSmallListVM))
                {
                    stepItemVM.IsHidden = true;
                }
                if (appllicationType == APPLICATION_TYPE.Temporary && itemType.Type == nameof(ForeignerVM))
                // (itemType.Type == nameof(ForeignerVM) || itemType.Type == nameof(EmploymentVM))                   )
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

        public async Task SaveDocument(Guid applicationId, DocumentVM document)
        {
            var application = await GetApplication(applicationId);
            var documents = application.GetDocuments();
            if (documents?.Documents.Any() != true)
            {
                var applicationType = application.GetApplicationType();
                var applicant = application.GetApplicant();
                documents = await GetDocumentTypes(applicationId, applicationType!.PermitType, applicant!.ApplicantType);
            }
            var doc = documents.Documents
                               .Where(x => x.DocumentTypeCode == document.DocumentTypeCode)
                               .Where(x => x.ForeignerSmallId == document.ForeignerSmallId)
                               .FirstOrDefault();
            if (doc == null)
            {
                documents.Documents.Add(document);
            }
            else
            {
                doc.ApplicationId = applicationId;
                doc.Id = document.Id;
                doc.CmisId = document.CmisId;
                doc.PortalId = document.PortalId;
                doc.HasMultipleFile = document.HasMultipleFile;
                doc.HasTitle = document.HasTitle;
                doc.Title = document.Title;
                doc.ForeignerLabel = document.ForeignerLabel;
                doc.ForeignerSmallId = document.ForeignerSmallId;
                doc.FileName = document.FileName;
                doc.FileUrl = document.FileUrl;
                doc.FileHash = document.FileHash;
            }
            await SaveDataItem(
                applicationId,
            nameof(DocumentsVM),
                JsonConvert.SerializeObject(documents));
        }

        public async Task RemoveDocument(Guid applicationId, Guid portalId, bool remove)
        {
            var application = await GetApplication(applicationId);
            var documents = application.GetDocuments();
            if (documents?.Documents.Any() != true)
            {
                var applicationType = application.GetApplicationType();
                var applicant = application.GetApplicant();
                documents = await GetDocumentTypes(applicationId, applicationType!.PermitType, applicant!.ApplicantType);
            }
            if (remove)
            {
                documents.Documents = documents.Documents
                                               .Where(x => x.PortalId != portalId)
                                               .ToList();
            }
            else
            {
                var doc = documents.Documents
                                   .Where(x => x.PortalId == portalId)
                                   .FirstOrDefault();
                doc.Id = null;
                doc.CmisId = string.Empty;
                doc.FileUrl = string.Empty;
                doc.FileName = string.Empty;
                doc.Title = string.Empty;
            }
            await SaveDataItem(
                applicationId,
            nameof(DocumentsVM),
                JsonConvert.SerializeObject(documents));
        }
        public async Task<DocumentResultVM> UploadFile(DocumentVM model)
        {
            string? foreignerId = null;
            if (model.ForeignerSmallId != null)
            {
                var application = await GetApplication(model.ApplicationId);
                foreignerId = application?.GetForeignerSmallList()?.Items
                                           .Where(x => x.Id == model.ForeignerSmallId)
                                           .Select(x => x.PassportNumber)
                                           .FirstOrDefault();
            }
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
                ForeignerId = foreignerId,
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
                HasMultipleFile = model.HasMultipleFile!,
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
        public List<DocumentVM> CreateOtherDocument(Guid applicationId, DocumentsVM? appDocs, Guid? foreignerId)
        {
            var result = new List<DocumentVM>();
            if (appDocs != null)
            {
                var documents = appDocs?.Documents
                                    .Where(x => x.DocumentTypeCode == DocumentType.Other)
                                    .Where(x => x.ForeignerSmallId == foreignerId)
                                    .Where(x => !string.IsNullOrEmpty(x.CmisId))
                                    .ToList();
                foreach (var document in documents)
                {
                    result.Add(document);
                }
            }
            result.Add(new DocumentVM
            {
                ApplicationId = applicationId,
                DocumentTypeCode = DocumentType.Other,
                DocumentType = localizer["DocumentTypeOther"],
                DocumentCategoryCode = "1",
                HasMultipleFile = true,
                HasTitle = true,
            });
            return result;
        }
        public DocumentVM GetFromDocumentsOrCreate(Guid applicationId, DocumentsVM? appDocs, Guid? foreignerId, ServiceDocumentsNomenclature documentType)
        {
            var document = appDocs?.Documents
                                   .Where(x => x.DocumentTypeCode == documentType.DocumentTypeValue)
                                   .Where(x => x.ForeignerSmallId == foreignerId)
                                   .FirstOrDefault();
            if (document != null)
            {
                return document;
            }
            return new DocumentVM
            {
                ApplicationId = applicationId,
                DocumentTypeCode = documentType.DocumentTypeValue,
                DocumentType = documentType.DocumentTypeName,
                DocumentCategoryCode = documentType.DocumentCategoryValue,
                DocumentCategory = documentType.DocumentCategoryName,
                IsMandatory = documentType.IsMandatory,
            };

        }
        public async Task<DocumentsVM> GetDocumentTypes(Guid applicationId, string permitType, string applicantType)
        {
            var application = await GetApplication(applicationId);
            var foreignerSmallList = application.GetForeignerSmallList();
            var applicationType = application.GetApplicationType();
            var appDocs = application.GetDocuments();
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
                var docsOnForeigner = new string[] { };
                if (permitType == PERMIT_TYPE.Temporary)
                    docsOnForeigner = ["4", "5", "8"];
                var documentTypesAll = types.Documents.Where(x => !docsOnForeigner.Contains(x.DocumentTypeValue))
                                                      .Where(x => x.DocumentTypeValue != "98")
                                                      .Where(x => x.DocumentTypeValue != DocumentType.Other)
                                            .ToList();
                foreach (var documentType in documentTypesAll)
                {
                    if (
                        (documentType.DocumentCategoryValue == "1" && documentType.DocumentTypeValue == "13") ||
                        (documentType.DocumentCategoryValue == "2" && documentType.DocumentTypeValue == "9") ||
                        (documentType.DocumentCategoryValue == "3" && documentType.DocumentTypeValue == "13") ||
                        (documentType.DocumentCategoryValue == "4" && documentType.DocumentTypeValue == "15")
                        )
                    {
                        var foreigner = application.GetForeigner();
                        if (foreigner?.TypeIdentifier == FOREIGNER_TYPE_IDENTIFIER.External)
                            documentType.IsMandatory = false;
                    }
                    documents.Documents.Add(GetFromDocumentsOrCreate(applicationId, appDocs, null, documentType));
                }
                if (foreignerSmallList != null && applicationType?.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
                {
                    var onForeignerDocuments = new List<DocumentVM>();
                    var documentTypes = types.Documents.Where(x => docsOnForeigner.Contains(x.DocumentTypeValue))
                                             .ToList();
                    foreach (var foreigner in foreignerSmallList.Items)
                    {
                        foreach (var documentType in documentTypes)
                        {
                            var document = GetFromDocumentsOrCreate(applicationId, appDocs, foreigner.Id, documentType);
                            document.ForeignerLabel = foreigner.NameCyrilic;
                            document.ForeignerSmallId = foreigner.Id;
                            document.Title = foreigner.NameCyrilic;
                            onForeignerDocuments.Add(document);
                        };
                    }
                    documents.Documents.AddRange(onForeignerDocuments);
                }
                documents.Documents.AddRange(CreateOtherDocument(applicationId, appDocs, null));
            }
            else
            {
                logger.LogError(types.Status + Environment.NewLine + types.Message);
            }
            return documents;
        }
        public async Task<List<ServiceDocumentsNomenclature>> GetDocumentTypesNomenclature()
        {
            var result = new List<ServiceDocumentsNomenclature>();
            var types = await client.GetDocumentsNomenclatureAsync(new GetDocumentsNomenclature_input
            {
                ShowAll = true,
                ApplicantTypeValue = string.Empty,
                PermitTypeValue = string.Empty,
            });
            if (types.Status == "OK")
            {
                result = types.Documents.ToList();
            }
            else
            {
                logger.LogError(types.Status + Environment.NewLine + types.Message);
            }
            return result;
        }
        public ServiceEntity ForeignerToCoreApplicant(ApplicantVM model)
        {
            var addresses = model.Foreigner.GetAddresses();
            foreach (var address in addresses.Items)
            {
                address.Kind = ADDRESSE_TYPE.Correspondence;
            }
            return new ServiceEntity
            {
                Type = model.ApplicantType,
                IdentifierNumber = model.Lnch,
                Name = model.Foreigner.NameCyrilic,
                TypeIdentifier = IDENTIFIER.Lnch,
                TypeIdentifierRepresentative = string.Empty,
                Fid = string.Empty,
                Address = ToCoreAddresses(addresses),
                Employer = new ServiceEmployer(),
                ContactInfo = ToCoreContactInfoList(model.Foreigner.Contacts),
            };
        }
        public ApplicantVM ToVMApplicant(ServiceEntity applicant)
        {
            var applicantVM = new ApplicantVM();
            applicantVM.ApplicantType = applicant.Type;
            if (applicant.Type == ENTITY_TYPE.Foreigner)
            {
                applicantVM.Lnch = applicant.IdentifierNumber;
                applicantVM.Foreigner.NameCyrilic = applicant.Name;
                applicantVM.Foreigner.Address = ToVMAddress(applicant.Address.FirstOrDefault()!);
                applicantVM.Foreigner.Contacts = ToVMContactInfoList(applicant.ContactInfo);
            }
            else
            {
                applicantVM.Egn = applicant.IdentifierNumber;
                applicantVM.Person.Name = applicant.Name;
                applicantVM.Person.Address = ToVMAddress(applicant.Address.First());
                applicantVM.Person.Contacts = ToVMContactInfoList(applicant.ContactInfo);
                //applicantVM.Person.ApplicantRole = string.IsNullOrEmpty(applicant.Employer?.Name) ? ApplicationRole.Person : ApplicationRole.Representative;
                applicantVM.Person.ApplicantRole = applicant.TypeIdentifierRepresentative;
                if (!string.IsNullOrEmpty(applicant.Employer?.IdentifierNumber))
                {
                    applicantVM.Person.Employer = ToVMEmployer(applicant.Employer);
                }
            }
            return applicantVM;
        }
        public ApplicationTypeVM ToVMApplicationType(ServiceApplication application)
        {
            var applicationType = new ApplicationTypeVM
            {
                ApplicationTypeCode = application.ApplicationType,
                PermitType = application.PermitType,
            };
            if (string.IsNullOrEmpty(applicationType.ApplicationTypeCode))
            {
                applicationType.ApplicationTypeCode = applicationType.PermitType == PERMIT_TYPE.Temporary ? APPLICATION_TYPE.Temporary : APPLICATION_TYPE.Permanent;
            }
            return applicationType;
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
            if (model.Person.ApplicantContactAddressIsSame == YESNO_TYPE.No)
            {
                addresses.Items.Add(model.Person.Address);
            }
            else
            {
                var empAddresses = model.Person.Employer.GetAddresses();
                var empAddress = empAddresses.Items.Where(x => x.Kind == ADDRESSE_TYPE.Correspondence).FirstOrDefault();
                addresses.Items.Add(empAddress);
            }
            // TODO: представител на фирма
            return new ServiceEntity
            {
                Type = model.ApplicantType,
                IdentifierNumber = model.Egn,
                Name = model.Person.Name,
                TypeIdentifier = IDENTIFIER.Egn,
                TypeIdentifierRepresentative = model.Person.ApplicantRole,
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
            if (address == null)
            {
                return new AddressVM();
            }
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
                ExpirationDate = OffsetToDateTime(document.ExpirationDate),
                IssueDate = OffsetToDateTime(document.IssueDate),
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
            return date?.LocalDateTime ?? DateTime.MinValue;
        }
        private DateTime? OffsetToNullDateTime(DateTimeOffset? date)
        {
            return date?.LocalDateTime;
        }
        public ServiceForeigner ToCoreForeigner(ApplicationVM model)
        {
            var foreigner = model.GetForeigner();
            var employment = model.GetEmployment() ?? new EmploymentVM();
            if (foreigner != null)
            {
                string? birthDateMonth = null;
                string? birthDateYear = null;
                if (foreigner.BirthDateTypeInput == BIRTH_DATE_TYPE_INPUT.YYYYMM)
                {
                    birthDateYear = foreigner.BirthMonth.Year.ToString();
                    birthDateMonth = foreigner.BirthMonth.Month.ToString();
                }
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
                    BirthDateTypeInput = foreigner.BirthDateTypeInput,
                    BirthDate = foreigner.BirthDateTypeInput == BIRTH_DATE_TYPE_INPUT.YYYYMMDD ? foreigner.BirthDate : null,
                    BirthDateMonth = birthDateMonth,
                    BirthDateYear = birthDateYear,
                    Address = ToCoreAddresses(foreigner.GetAddresses()),
                    ContactInfo = ToCoreContactInfoList(foreigner.Contacts),
                    Identifiers = foreigner.PersonIdDocuments.Items.Select(x => ToCoreIdDocument(x)).ToList(),
                    Gender = foreigner.Gender,
                    MaritalStatus = foreigner.MaritalStatus,
                    //   DurationOfEmploymentFrom = OldDateToNull(employment.DurationOfEmploymentFrom),
                    //   DurationOfEmploymentTo =OldDateToNull(employment.DurationOfEmploymentFrom.AddMonths(employment.DurationOfEmploymentMonth)),
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
                VisaExpirationDate = OffsetToNullDateTime(foreigner.VisaExpirationDate),
                VisaSerialNumber = foreigner.VisaSerialNumber,
                VisaType = foreigner.VisaType,
                Nationality = foreigner.Nationality,
                CityОfBirth = foreigner.CityОfBirth,
                BirthDate = OffsetToDateTime(foreigner.BirthDate),
                Address = ToVMAddress(foreigner.Address.FirstOrDefault()!),
                Contacts = ToVMContactInfoList(foreigner.ContactInfo),
                PersonIdDocuments = new PersonIdDocumentListVM
                {
                    Items = foreigner.Identifiers.Select(x => ToVMIdDocument(x)).ToList()
                },
                Gender = foreigner.Gender,
                MaritalStatus = foreigner.MaritalStatus,
                EntryDate = OffsetToNullDateTime(foreigner.EntryDate),
                EntryPoint = foreigner.EntryPoint,
            };
        }

        public EmploymentVM ToVMEmployment(ServiceApplication application)
        {
            var foreigner = application.Subject;
            //var period = foreigner.DurationOfEmploymentTo - foreigner.DurationOfEmploymentFrom;
            //var months = 0;
            //if (period != null)
            //{
            //    var days = period.Value.Days;
            //    months = days / 30;
            //}
            return new EmploymentVM
            {
                DurationOfEmploymentFrom = OffsetToDateTime(foreigner.DurationOfEmploymentFrom),
                DurationOfEmploymentMonth = application.ContractDuration,
                Position = foreigner.Position,
                EmploymentReason = foreigner.EmploymentReason,
                SpecialityCode = application.NkpdPosition,
                Speciality = application.Specialty,
                Type = application.ContractContinuation ? YESNO_TYPE.Yes : YESNO_TYPE.No,
                EmployerChange = application.EmployerChange ? YESNO_TYPE.Yes : YESNO_TYPE.No,
                Qualification = application.Qualification,
                AddressObject = application.WorplaceSite,
                AddressIsSame = application.EmploymentAddressMatch ? YESNO_TYPE.Yes : YESNO_TYPE.No,
                Address = !application.EmploymentAddressMatch ? ToVMAddress(application.EmploymentAddress) : null!,
                EducationType = application.EducationType,
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

        public List<ServiceDocument> ToCoreDocuments(DocumentsVM documents, ForeignerSmallListVM? foreignerSmallList)
        {
            var result = new List<ServiceDocument>();
            foreach (var document in documents.Documents.Where(x => x.DocumentTypeCode != "99" || !string.IsNullOrEmpty(x.CmisId)))
            {
                var documentTitle = document.Title;
                if (string.IsNullOrEmpty(documentTitle))
                {
                    documentTitle = document.DocumentType;
                }
                string? foreignerId = null;
                if (document.ForeignerSmallId != null && foreignerSmallList != null)
                {
                    foreignerId = foreignerSmallList.Items.Where(x => x.Id == document.ForeignerSmallId).Select(x => x.PassportNumber).FirstOrDefault();
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
                    ForeignerId = foreignerId
                });
            }
            return result;
        }

        public async Task<DocumentsVM> ToVMDocuments(ServiceApplication application)
        {
            var result = new DocumentsVM
            {
                ApplicantType = application.Applicant.Type,
                PermitType = application.PermitType,
            };
            var documentTypes = await GetDocumentTypesNomenclature();
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
                    Title = document.Title
                };
                documentVM.DocumentType = documentTypes
                    .Where(x => x.DocumentTypeValue == documentVM.DocumentTypeCode &&
                                x.DocumentCategoryValue == documentVM.DocumentCategoryCode)
                    .Select(x => x.DocumentTypeName)
                    .FirstOrDefault() ?? string.Empty;
                if (documentVM.DocumentTypeCode == "99")
                {
                    if (!string.IsNullOrEmpty(documentVM.Title))
                    {
                        var separator = !string.IsNullOrEmpty(documentVM.DocumentType) ? ": " : string.Empty;
                        documentVM.DocumentType += $"{separator}{documentVM.Title}";
                    }
                    documentVM.Title = string.Empty;
                }
                if (documentVM.DocumentTypeCode == "98")
                {
                    if (documentVM.Title.StartsWith("Заявление") == true)
                    {
                        documentVM.DocumentType = localizer["DocumentPrintForm"];
                        documentVM.FileName = string.Format(localizer["DocumentPrintFormFileName"], application.ApplicationId);
                    }
                    else
                    {
                        var separator = !string.IsNullOrEmpty(documentVM.DocumentType) ? ": " : string.Empty;
                        if (!string.IsNullOrEmpty(documentVM.Title))
                            documentVM.DocumentType += $"{separator}{documentVM.Title}";
                        documentVM.Title = string.Empty;
                    }
                    documentVM.Title = string.Empty;
                }
                if (string.IsNullOrEmpty(documentVM.DocumentType))
                {
                    documentVM.DocumentType = documentVM.Title;
                }
                if (string.IsNullOrEmpty(documentVM.FileName))
                {
                    documentVM.FileName = documentVM.Title;
                    documentVM.Title = string.Empty;
                }

                documentVM.FileName = documentVM.FileName?.Replace(" ", "_") ?? string.Empty;
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
                Documents = ToCoreDocuments(model.GetDocuments()!, model.GetForeignerSmallList()),
                AdditionalInfo = applicationInfo?.AdditionalInfo,
            };
            var employment = model.GetEmployment();
            if (applicationType?.ApplicationTypeCode == APPLICATION_TYPE.Permanent)
            {
                application.ContractContinuation = employment?.Type == YESNO_TYPE.Yes;
                application.EmployerChange = employment?.EmployerChange == YESNO_TYPE.Yes;
                application.ContractDuration = employment?.DurationOfEmploymentMonth ?? 0;
                application.Position = employment?.Position;
                application.Qualification = employment?.Qualification;
                application.Specialty = employment?.Speciality;
                application.EducationType = employment?.EducationType;
                application.NkpdPosition = employment?.SpecialityCode;
                application.EmploymentAddress = employment?.AddressIsSame == YESNO_TYPE.No ? ToCoreAddress(employment?.Address) : application.Employer.Address.FirstOrDefault();
                application.Subject = ToCoreForeigner(model);
                application.EmploymentAddressMatch = employment?.AddressIsSame == YESNO_TYPE.Yes;
            }
            if (applicationType?.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                var foreignerSmallList = model.GetForeignerSmallList();
                application.PermitType = PERMIT_TYPE.Temporary;
                application.SubjectLists = foreignerSmallList.Items.Select(x => ToCoreForeignerSmall(x)).ToList();
                application.EmploymentAddressMatch = employment?.AddressIsSame == YESNO_TYPE.Yes;
                application.EmploymentAddress = employment?.AddressIsSame == YESNO_TYPE.No ? ToCoreAddress(employment?.Address) : application.Employer.Address.FirstOrDefault();
                application.WorplaceSite = employment?.AddressObject;
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
        public async Task<(string, DateTime, ApplicationMessage)> SignRequest(string request, string? permitType, Guid applicationId)
        {
            (string signature, byte[] tsr, DateTime timestamp) = await SignRequestTimestamp(request, permitType);
            var applicationMessage = new ApplicationMessage
            {
                ApplicationId = applicationId,
                DateWrt = DateTime.UtcNow,
                RegistrationData = request,
                RegistrationTimeStamp = tsr,
                RegistrationDataSignature = signature,
            };
            await repo.AddAsync(applicationMessage);
            return (signature, timestamp, applicationMessage);
        }

        private async Task<(string signature, byte[] tsr, DateTime timestamp)> SignRequestTimestamp(string request, string? permitType)
        {
            var configSection = $"SignerPermit{permitType}";
            TimestampClientOptions tsOptions = new TimestampClientOptions()
            {
                TimestampEndpoint = configuration.GetValue<string>($"{configSection}:TimestampUrl"),
                Token = configuration.GetValue<string>($"{configSection}:Token"),
                Username = configuration.GetValue<string>($"{configSection}:TimestampUsername"),
                Password = configuration.GetValue<string>($"{configSection}:TimestampPassword"),
            };

            return await signerService.StampIt(
                Encoding.UTF8.GetBytes(request),
                tsOptions,
                configuration.GetValue<string>($"{configSection}:CertificateFile") ?? string.Empty,
                configuration.GetValue<string>($"{configSection}:CertificatePassword"));
        }

        public async Task<(string, string)> SendApplication(Guid applicationId)
        {
            var applicationVM = await GetApplication(applicationId);
            var application_input = await ToCoreApplication(applicationVM);
            string request = client.SerializeForSignature(application_input.Application);
            (string signature, DateTime timestamp, var applicationMessage) = await SignRequest(request, applicationVM?.GetApplicationType()?.PermitType, applicationId);
            var status = string.Empty;
            var message = string.Empty;
            application_input.Application.RegistrationData = request;
            application_input.Application.RegistrationDataSignature = signature;
            application_input.Application.RegistrationTimeStamp = timestamp.ToString(FormattingConstant.DateFormat);
            var application = await repo.All<Application>().Where(x => x.Id == applicationId).FirstAsync();
            try
            {
                var result = await client.CreateApplicationAsync(application_input);
                status = result.Status;
                message = result.Message;
                if (status == "OK")
                {
                    application.Status = ApplicationStatus.Send;
                    application.ApplyNumber = result.ApplicationId;
                }
            }
            catch (Exception ex)
            {
                status = "Exception";
                message = ex.Message + Environment.NewLine + ex.StackTrace;
            }
            applicationMessage.ResponseStatus = status;
            applicationMessage.ResponseMessage = message;

            if (status != "OK")
            {
                logger.LogError(status + Environment.NewLine + message);
            }
            await repo.SaveChangesAsync();
            return (status, status == "OK" ? localizer["SendApplication"] + application.ApplyNumber : message);
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

        public async Task SetStatusNone(Guid applicationId)
        {
            var application = await repo.All<Application>().Where(x => x.Id == applicationId).FirstOrDefaultAsync();
            if (application != null)
            {
                application.Status = ApplicationStatus.None;
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
            application.SetData(ToVMEmployer(responce.Application.Employer));
            application.SetData(ToVMEmployment(responce.Application));
            application.SetData(ToVMApplicationInfo(responce.Application));
            application.SetData(await ToVMDocuments(responce.Application));
            if (applicationType.ApplicationTypeCode == APPLICATION_TYPE.Permanent)
            {
                application.SetData(ToVMForeigner(responce.Application.Subject));
                application.ApplicationItems = application.ApplicationItems
                                                          .Where(x => !(x.Data is ForeignerSmallListVM))
                                                          .ToList();
            }
            if (applicationType.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
            {
                application.SetData(await ToVMForeignerSmallList(responce.Application.SubjectLists));
                application.ApplicationItems = application.ApplicationItems
                                                          .Where(x => !(x.Data is ForeignerVM))
                                                          .ToList();
                application.ApplicationItems.ForEach(x => x.IsTemporary = true);
            }
            application.ApplicationItems.ForEach(x => x.PermitType = applicationType.PermitType);
            return application;
        }
        public async Task<List<ApplicationListItemVM>> GetApplicationList()
        {
            //var statuses = string.Join(Environment.NewLine,(await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.EXTERNAL_STATUS))
            //               .Select(x => $"{x.Value} {x.Text}")
            //               .ToList());

            //var internalstatuses = string.Join(Environment.NewLine, (await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.INTERNAL_STATUS))
            //               .Select(x => $"{x.Value} {x.Text}")
            //               .ToList());
            var responce = await client.ListApplicationsAsync(new ListApplications_input { IdentifierNumber = userContext.Pid, RecordsPerPage = 100000, Bulstat = userContext.Eik });
            var result = new List<ApplicationListItemVM>();
            var forSelfDenial = await nomenclatureService.GetStatusesFor(ParamTypes.StatusForSelfDenail);
            var forComplaint = (await nomenclatureService.GetStatusesFor(ParamTypes.StatusForComplaint));
            var forUpdate = await nomenclatureService.GetStatusesFor(ParamTypes.StatusForUpdate);
            var hasComplaint = await nomenclatureService.GetStatusesFor(ParamTypes.InternalStatusHasComplaint);
            var forPay = await nomenclatureService.GetStatusesFor(ParamTypes.StatusForPay);
            if (responce.Status == "OK")
            {
                var paymentUrl = configuration.GetValue<string>($"Payment:BaseUrl");
                foreach (var item in responce.Applications)
                {
                    var sNameCyrilic = item.SubjectNameCyrilic.ToList();
                    var sBirthDate = item.SubjectBirthDate?.ToList();
                    var sNationality = item.SubjectNationality?.ToList();
                    var applicationListItem = new ApplicationListItemVM
                    {
                        ApplicationId = item.ApplicationId,
                        ApplicationNumber = item.ApplicationId,
                        ForeignerLNCH = item.SubjectFid,
                        PermitTypeCode = item.PermitType,
                        StatusCode = item.Status,
                        EntryDate = OffsetToNullDateTime(item.RegistrationDate),
                        EmployerName = item.EmployerName,
                    };

                    applicationListItem.PaymentAccessCode = forPay.Contains(item.Status) && !string.IsNullOrEmpty(item.PaymentAccessCode)
                                                            ? paymentUrl + item.PaymentAccessCode
                                                            : string.Empty;
                    applicationListItem.ForComplaint = forComplaint.Contains(item.Status) && !hasComplaint.Contains(item.InternalStatus);
                    applicationListItem.ForSelfDenial = forSelfDenial.Contains(item.Status);
                    applicationListItem.ForUpdate = forUpdate.Contains(item.Status);
                    applicationListItem.PermitType = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.PERMIT_TYPE, applicationListItem.PermitTypeCode);
                    applicationListItem.Status = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.EXTERNAL_STATUS, applicationListItem.StatusCode);
                    if (hasComplaint.Contains(item.InternalStatus))
                    {
                        applicationListItem.Remark += localizer["HasActiveComplaint"];
                    }

                    result.Add(applicationListItem);
                    for (int i = 0; i < sNameCyrilic.Count; i++)
                    {
                        applicationListItem.ForeignerName += sNameCyrilic[i] + "<br>";
                        applicationListItem.ForeignerBirthDate += (sBirthDate == null || sBirthDate.Count() <= i ?
                                                 string.Empty :
                                                 sBirthDate[i]?.ToLocalTime().ToString(FormattingConstant.NormalDateFormat)) +
                                                 "<br>";
                        applicationListItem.ForeignerNationality = (sNationality == null ? string.Empty : sNationality[i]) + "<br>";

                    }
                }
            }
            else
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            return result;
        }

        public async Task<GridResponseModel> GetComplaintList(string filterJson, int? inPage, long? inPageSize)
        {
            var filter = DeserializeDataObject<ComplaintFilterVM>(filterJson);
            var responce = await client.ListComplaintsAsync(new ListComplaints_input { IdentifierNumber = userContext.Pid, Bulstat = userContext.Eik });
            var result = new List<ComplaintListItemVM>();
            var applications = await GetApplicationList();
            if (responce.Status == "OK")
            {

                foreach (var item in responce.Complaints)
                {
                    var application = applications.Where(x => x.ApplicationId == item.ApplicationId).FirstOrDefault();
                    var complaintListItem = new ComplaintListItemVM
                    {
                        ComplaintNumber = item.ComplaintId,
                        ApplicationNumber = item.ApplicationId,
                        ComplaintName = item.ComplainantName,
                        Status = item.Status,
                        StatusDate = OffsetToNullDateTime(item.StatusDate),
                        ComplaintDate = OffsetToNullDateTime(item.StatusDate),
                        ForeignerBirthDate = application?.ForeignerBirthDate,
                        ForeignerName = application?.ForeignerName,
                        ForeignerLNCH = application?.ForeignerLNCH,
                    };
                    complaintListItem.Status = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.EXTERNAL_STATUS, complaintListItem.Status);
                    result.Add(complaintListItem);
                }
            }
            else
            {
                logger.LogError(responce.Status + Environment.NewLine + responce.Message);
            }
            result = result.Where(x => string.IsNullOrEmpty(filter.ComplaintNumber) || x.ComplaintNumber == filter.ComplaintNumber)
                           .Where(x => filter.FromDate == null || filter.FromDate <= x.ComplaintDate)
                           .Where(x => filter.ToDate == null || x.ComplaintDate <= filter.ToDate)
                           .ToList();

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
                       .Where(x => string.IsNullOrEmpty(country) || x.ForeignerNationality?.Contains(country) == true)
                       .Where(x => filter.FromDate == null || filter.FromDate <= x.EntryDate)
                       .Where(x => filter.ToDate == null || x.EntryDate <= filter.ToDate)
                       .Where(x => filter.BirthDate == null || x.ForeignerBirthDate?.Contains(filter.BirthDate?.ToString(FormattingConstant.NormalDateFormat)) == true)
                       .OrderByDescending(x => x.EntryDate)
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
                    EntryDate = application.DateWrt,
                };
                var foreignerType = applicationItemTypes.Where(x => x.Type == nameof(ForeignerVM)).FirstOrDefault();
                var foreignerItem = application.ApplicationItems.Where(x => x.ItemTypeId == foreignerType!.Id).FirstOrDefault();
                if (foreignerItem != null)
                {
                    var foreigner = DeserializeDataObject<ForeignerVM>(foreignerItem.DataContent);
                    applicationListItem.ForeignerName = foreigner.NameCyrilic;
                }
                var appTypeType = applicationItemTypes.Where(x => x.Type == nameof(ApplicationTypeVM)).FirstOrDefault();
                var appTypeItem = application.ApplicationItems.Where(x => x.ItemTypeId == appTypeType!.Id).FirstOrDefault();
                if (appTypeItem != null)
                {
                    var appType = DeserializeDataObject<ApplicationTypeVM>(appTypeItem.DataContent);
                    if (appType.ApplicationTypeCode == APPLICATION_TYPE.Temporary)
                    {
                        foreignerType = applicationItemTypes.Where(x => x.Type == nameof(ForeignerSmallListVM)).FirstOrDefault();
                        var foreignerSmallItems = application.ApplicationItems.Where(x => x.ItemTypeId == foreignerType!.Id).FirstOrDefault();
                        if (foreignerSmallItems != null)
                        {
                            var foreignerList = DeserializeDataObject<ForeignerSmallListVM>(foreignerSmallItems.DataContent);
                            if (foreignerList != null)
                            {
                                foreach (var foreigner in foreignerList.Items)
                                {
                                    applicationListItem.ForeignerName += foreigner.NameCyrilic + "<br>";
                                }
                            }
                        }
                        appType.PermitType = PERMIT_TYPE.Temporary;
                    }
                    applicationListItem.PermitType = await nomenclatureService.GetNomenclatureText(NomenclatureTypes.PERMIT_TYPE, appType.PermitType);
                }
                applicationList.Add(applicationListItem);
            }
            var query = applicationList.OrderByDescending(x => x.EntryDate).AsQueryable();
            return CalcGridResponseModel<ApplicationListItemVM>(query, inPage, inPageSize);
        }

        public async Task<(string, DateTime, UpdateMessage)> SignRequestUpdate(string request, string? permitType, int sourceType, Guid sourceId)
        {

            (string signature, byte[] tsr, DateTime timestamp) = await SignRequestTimestamp(request, permitType);
            var updateMessage = new UpdateMessage
            {
                SourceId = sourceId,
                SourceTypeId = sourceType,
                DateWrt = DateTime.UtcNow,
                RegistrationData = request,
                RegistrationTimeStamp = tsr,
                RegistrationDataSignature = signature,
            };
            await repo.AddAsync(updateMessage);
            return (signature, timestamp, updateMessage);
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
            var body = new CreateComplaint_input { Complaint = ToCoreComplaint(complaintVm) };
            var app = await client.GetApplicationAsync(new GetApplication_input { ApplicationId = complaint.ApplyNumberFrom });
            string? permitType = app.Application.PermitType;
            string request = client.SerializeForSignatureUpdate(body);

            (string signature, DateTime timestamp, UpdateMessage updateMessage) = await SignRequestUpdate(request, permitType, SourceType.Complaint, complaint.Id);
            await repo.SaveChangesAsync();
            body.Complaint.ComplaintDataSignature = signature;
            body.Complaint.ComplaintTimeStamp = timestamp.ToString(FormattingConstant.DateFormat);
            var response = await client.CreateComplaintAsync(body);

            var result = true;
            var message = $"Подадена е жалба {response.ComplaintId}";
            if (response.Status != "OK")
            {
                message = response.Message;
                result = false;
            }
            complaint.ComplaintNumber = response.ComplaintId;
            updateMessage.ResponseStatus = response.Status;
            updateMessage.ResponseMessage = response.Message;
            await repo.SaveChangesAsync();
            return (result, message);
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
            var app = await client.GetApplicationAsync(new GetApplication_input { ApplicationId = applicationUpdateVm.ApplicationNumber });
            string? permitType = app.Application.PermitType;
            var body = new UpdateApplication_input
            {
                ApplicationId = applicationUpdateVm.ApplicationNumber,
                Documents = ToCoreDocuments(applicationUpdateVm.Documents, null),
                InternalStatus = app.Application.InternalStatus,
                ApplicantName = applicationUpdateVm.Applicant.ApplicantName()
            };
            string request = client.SerializeForSignatureUpdate(body);
            (string signature, DateTime timestamp, UpdateMessage updateMessage) = await SignRequestUpdate(request, permitType, SourceType.ApplicationUpdate, applicationUpdateVm.Id);

            applicationUpdate.ApplyNumberFrom = applicationUpdateVm.ApplicationNumber;
            applicationUpdate.DateWrt = DateTime.UtcNow;
            applicationUpdate.DataContent = JsonConvert.SerializeObject(applicationUpdateVm);
            applicationUpdate.UserId = userContext.Id;
            await repo.SaveChangesAsync();
            body.RegistrationDataSignature = signature;
            body.RegistrationTimeStamp = timestamp.ToString(FormattingConstant.DateFormat);

            var response = await client.UpdateApplicationAsync(body);
            var result = true;
            var message = $"Подадена е промяна по заявление {applicationUpdateVm.ApplicationNumber}";
            if (response.Status != "OK")
            {
                result = false;
                message = response.Message;

            }
            applicationUpdate.Status = 99;
            updateMessage.ResponseStatus = response.Status;
            updateMessage.ResponseMessage = response.Message;

            await repo.SaveChangesAsync();
            return (result, message);
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
            string request = client.SerializeForSignatureUpdate(rejectionVm);
            var app = await client.GetApplicationAsync(new GetApplication_input { ApplicationId = rejectionVm.ApplicationNumber });
            string? permitType = app.Application.PermitType;
            (string signature, DateTime timestamp, UpdateMessage updateMessage) = await SignRequestUpdate(request, permitType, SourceType.SelfDenial, rejectionVm.Id);

            rejection.ApplyNumberFrom = rejectionVm.ApplicationNumber;
            rejection.DateWrt = DateTime.UtcNow;
            rejection.DataContent = JsonConvert.SerializeObject(rejectionVm);
            rejection.UserId = userContext.Id;
            await repo.SaveChangesAsync();

            var body = new UpdateApplication_input
            {
                InternalStatus = INTERNAL_STATUS.SelfDenial,
                ApplicationId = rejectionVm.ApplicationNumber,
                RegistrationDataSignature = signature,
                RegistrationTimeStamp = timestamp.ToString(FormattingConstant.DateFormat),
                Comment = rejectionVm.RejectionInfo,
                ApplicantName = rejectionVm.Applicant.ApplicantName()
            };
            var responce = await client.UpdateApplicationAsync(body);
            var result = true;
            var message = $"Подаден е самоотказ за {rejection.ApplyNumberFrom}";
            if (responce.Status != "OK")
            {
                result = false;
                message = responce.Message;
            }
            rejection.Status = int.Parse(INTERNAL_STATUS.SelfDenial);
            updateMessage.ResponseStatus = responce.Status;
            updateMessage.ResponseMessage = responce.Message;
            await repo.SaveChangesAsync();
            return (result, message);
        }
        public List<SeriesCountryList> GetPermitDataOnCountry(
            int pageCount,
            List<SelectListItem> countries,
            List<SelectListItem> permits,
            ICollection<PermitStatisticsDataItem> PermitStatisticsData)
        {
            countries = countries.Where(x => PermitStatisticsData.Any(s => s.Nationality == x.Text && permits.Any(p => p.Text == s.PermitType))).ToList();
            if (pageCount < 5)
                pageCount = 5;
            var result = new List<SeriesCountryList>();
            var length = countries.Count / pageCount;
            if (length * pageCount < countries.Count)
            {
                length++;
                if (length > 1)
                {
                    var forAdd = (length * pageCount - countries.Count);
                    for (int i = 0; i < forAdd; i++)
                    {
                        countries.Add(new SelectListItem { Value = "    ", Text = "    " });
                    }
                }
            }
            for (int i = 0; i < length; i++)
            {
                var item = new SeriesCountryList { Page = i };
                var pageCountries = countries.Skip(i * pageCount).Take(pageCount);
                item.Categories = pageCountries.Select(x => x.Text).ToList();
                foreach (var permit in permits)
                {
                    var data = new List<int>();
                    foreach (var country in pageCountries)
                    {
                        var statistic = PermitStatisticsData
                             .Where(x => x.Nationality == country.Text &&
                                         x.PermitType == permit.Text)
                             .FirstOrDefault();
                        data.Add(statistic?.NumberOfApplications ?? 0);
                    }
                    item.Series.Add(new SeriesCountryItem
                    {
                        Name = permit.Text,
                        Data = data.ToArray(),
                        Color = GetColorOnPermit(permit.Value)
                    });
                }
                result.Add(item);
            }
            return result;
        }

        public SeriesCountryList GetPermitDataOnCountryTop10(
            List<SelectListItem> permits,
            ICollection<PermitStatisticsDataItem> PermitStatisticsData)
        {
            var countries = PermitStatisticsData.GroupBy(x => x.Nationality)
                                           .Select(g => new CountryTopItem
                                           {
                                               Name = g.Key,
                                               Count = g.Sum(x => (int?)x.NumberOfApplications) ?? 0
                                           })
                                           .OrderByDescending(x => x.Count)
                                           .Take(10)
                                           .ToList();
            var item = new SeriesCountryList { Page = 0 };
            item.Categories = countries.Select(x => x.Name ?? string.Empty).ToList();
            foreach (var permit in permits)
            {
                var data = new List<int>();
                foreach (var country in countries)
                {
                    var statistic = PermitStatisticsData
                         .Where(x => x.Nationality == country.Name &&
                                     x.PermitType == permit.Text)
                         .FirstOrDefault();
                    data.Add(statistic?.NumberOfApplications ?? 0);
                }
                item.Series.Add(new SeriesCountryItem
                {
                    Name = permit.Text,
                    Data = data.ToArray(),
                    Color = GetColorOnPermit(permit.Value)
                });
            }
            return item;
        }
        public string GetColorOnPermit(string permitType)
        {
            if (permitType == PERMIT_TYPE.BlueCard)
                return "#0077B6";
            if (permitType == PERMIT_TYPE.UnifiedWorkPermit)
                return "#FFC946";
            if (permitType == PERMIT_TYPE.SeasonalWorkerPermit)
                return "#A0BF38";
            if (permitType == PERMIT_TYPE.IntracorporateTransfer)
                return "#FC5830";
            if (permitType == PERMIT_TYPE.Temporary)
                return "#E5F1F8";
            return string.Empty;
        }
        private int YearToInt(string Year)
        {
            var year = 0;
            int.TryParse(Year, out year);
            return year;
        }
        private void AddYear(Dictionary<int, ICollection<PermitStatisticsDataItem>> dictionary, string Year)
        {
            var year = YearToInt(Year);
            if (!dictionary.ContainsKey(year))
               dictionary.Add(year, null!);
        }
        private async Task<Dictionary<int, ICollection<PermitStatisticsDataItem>>> FillPermitData(DashboardFilter filter)
        {
            var result = new Dictionary<int, ICollection<PermitStatisticsDataItem>>();
            AddYear(result, filter.Year);
            AddYear(result, filter.YearCountry);
            AddYear(result, filter.YearCountryTop10);
            var fromYear = 0;
            int.TryParse(filter.recapFromYear, out fromYear);
            var toYear = 0;
            int.TryParse(filter.recapToYear, out toYear);
            for (int year = fromYear; year <= toYear; year++)
            {
                AddYear(result, year.ToString());
            }
            foreach (var year in result.Keys.ToList())
            {
                var response = await client.GetStatisticsAsync(new GetStatistics_input { Year = year });
                result[year] = response.PermitStatisticsData;
            }
            return result;
        }
        public async Task<SeriesVM> GetPermitData(DashboardFilter filter)
        {
            int pageSize = 300;
            SeriesVM result = new();
            var permits = await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.PERMIT_TYPE, false);
            var countries = (await nomenclatureService.GetNomenclatureDDL(NomenclatureTypes.COUNTRIES, false))
                                  .Where(x => (filter.Countries?.Count() ?? 0) == 0 || filter.Countries!.Any(t => t == x.Value))
                                  .ToList(); 
            var year = YearToInt(filter.Year);

            var dataDict = await FillPermitData(filter);
            result.PermitData = dataDict[year]
                                        .GroupBy(x => x.PermitType)
                                        .Select(g => new SeriesItem
                                        {
                                            Name = g.Key,
                                            Code = permits.Where(x => x.Text == g.Key).Select(x => x.Value).FirstOrDefault(),
                                            Y = g.Sum(x => (int?)x.NumberOfApplications) ?? 0,
                                            Color = GetColorOnPermit(permits.Where(x => x.Text == g.Key).Select(x => x.Value).FirstOrDefault()!)
                                        })
                                        .ToList();
            foreach (var permit in permits)
            {
                if (!result.PermitData.Any(x => x.Code == permit.Value))
                {
                    result.PermitData.Add(new SeriesItem
                    {
                        Name = permit.Text,
                        Code = permit.Value,
                        Y =  0,
                        Color = GetColorOnPermit(permit.Value)

                    });
                }
            }
            result.PermitData = result.PermitData
                                      .OrderBy(x => x.Code)
                                      .ToList();
            var yearCountry = YearToInt(filter.YearCountry);
            result.CountryData = GetPermitDataOnCountry(pageSize, countries, permits, dataDict[yearCountry]);
            result.CountryMaxPermit = dataDict[yearCountry].Max(x => (int?)x.NumberOfApplications) ?? 1;

            var yearCountryTop10 = YearToInt(filter.YearCountryTop10);
            result.CountryTop10Data = GetPermitDataOnCountryTop10(permits, dataDict[yearCountryTop10]);
            var fromYear = YearToInt(filter.recapFromYear);
            var toYear = YearToInt(filter.recapToYear);
            (result.YearData, result.YearMaxPermit) = await GetPermitDataOnYear(fromYear, toYear, permits, dataDict);
            return result;
        }

        public async Task<(SeriesCountryList, int)> GetPermitDataOnYear(
            int fromYear, 
            int toYear, 
            List<SelectListItem> permits,
            Dictionary<int, ICollection<PermitStatisticsDataItem>> dataDict)
        {
            var item = new SeriesCountryList { Page = 0 };
            int maxValue = 1;
            for (int year = fromYear; year <= toYear; year++)
            {
                item.Categories.Add(year.ToString());
            }
            foreach (var permit in permits)
            {
                var data = new List<int>();
                for (int year = fromYear; year <= toYear; year++)
                {
                    var permitStatisticsDataItem = dataDict[year];
                    var count = permitStatisticsDataItem
                                      .Where(x => x.PermitType == permit.Text)
                                      .Sum(x => (int?)x.NumberOfApplications) ?? 0;
                    if (maxValue < count )
                        maxValue = count;
                    data.Add(count);
                }
                item.Series.Add(new SeriesCountryItem
                {
                    Name = permit.Text,
                    Data = data.ToArray(),
                    Color = GetColorOnPermit(permit.Value)
                });
            }

            return (item, maxValue);
        }
        public void InitNewApplicant(ApplicantVM applicant)
        {
            applicant.UicType = userContext.PidType;
            if (userContext.PidType == "EGN")
            {
                applicant.Egn = userContext.Pid;
            }
            if (userContext.PidType == "LNCH")
            {
                applicant.Lnch = userContext.Pid;
            }
            applicant.Person.Employer.Identifier = userContext.Eik ?? string.Empty;

            applicant.Person.Employer.Contacts.AddNewIfEmpty();
            applicant.Person.Contacts.AddNewIfEmpty();
            applicant.Foreigner.Contacts.AddNewIfEmpty();
        }
        public SelfDenialVM InitNewSelfDenial(string applicationNumber)
        {
            var model = new SelfDenialVM
            {
                ApplicationNumber = applicationNumber,
            };
            InitNewApplicant(model.Applicant);
            return model;
        }
        public ComplaintVM InitNewComplaint(string applicationNumber)
        {
            var model = new ComplaintVM
            {
                ApplicationNumber = applicationNumber,
            };
            InitNewApplicant(model.Applicant);
            return model;
        }
        public ApplicationUpdateVM InitNewApplicationUpdate(string applicationNumber)
        {
            var model = new ApplicationUpdateVM
            {
                ApplicationNumber = applicationNumber
            };
            InitNewApplicant(model.Applicant);
            model.Documents.Documents.Add(CreateOtherDocument(model.Id, null, null).First());
            return model;
        }
    }
}
