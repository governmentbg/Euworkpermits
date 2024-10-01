using BlueCardPortal.Infrastructure.Data.Models.Application;
using BlueCardPortal.Infrastructure.Model;
using BlueCardPortal.Infrastructure.Model.Application;
using BlueCardPortal.Infrastructure.Model.ApplicationList;
using BlueCardPortal.Infrastructure.Model.Complaint;
using BlueCardPortal.Infrastructure.Model.SelfDenial;

namespace BlueCardPortal.Core.Contracts
{
    public interface IApplicationService : IBaseService
    {
        DocumentVM CreateOtherDocument(Guid applicationId);
        Task<DocumentResultVM> DownloadFile(string cmisId);
        Task<GridResponseModel> FilterApplicationList(string filterJson, List<ApplicationListItemVM> data, int? inPage, long? inPageSize);
        Task<ApplicationVM> GetApplication(Guid? applicationId, bool forPreview = false, bool isForRemote= false);
        Task<List<ApplicationListItemVM>> GetApplicationList();
        Task<GridResponseModel> GetApplicationListDraft(int? inPage, long? inPageSize);
        Task<ApplicationVM> GetApplicationRemote(string applicationId);
        Task<GridResponseModel> GetComplaintList(int? inPage, long? inPageSize);
        Task<DocumentsVM> GetDocumentTypes(Guid appliactionId, string permitType, string applicantType);
        Task<EmployerVM> GetEmployer(string uic);
        Task SaveApplicant(Guid applicationId, ApplicantVM applicant);
        Task SaveApplicationInfo(Guid applicationId, ApplicationInfoVM applicationInfo);
        Task SaveApplicationType(Guid applicationId, ApplicationTypeVM applicationType);
        Task<(bool, string)> SaveApplicationUpdate(ApplicationUpdateVM applicationUpdateVm);
        Task<(bool, string)> SaveComplaint(ComplaintVM complaintVm);
        Task SaveDocuments(Guid applicationId, DocumentsVM documents);
        Task SaveEmployer(Guid applicationId, EmployerVM employer);
        Task SaveEmployment(Guid applicationId, EmploymentVM employment);
        Task SaveForeigner(Guid applicationId, ForeignerVM foreigner);
        Task SaveForeignerSmallList(Guid applicationId, ForeignerSmallListVM foreignerList);
        Task<(bool, string)> SaveSelfDenial(SelfDenialVM rejectionVm);
        Task<(string, string)> SendApplication(Guid applicationId);
        Task SetStatusDraft(Guid applicationId);
        Task<DocumentResultVM> UploadFile(DocumentVM model);
    }
}
