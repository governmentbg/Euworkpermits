using BlueCardPortal.Infrastructure.Integrations.BlueCardCore.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.Application
{
    /// <summary>
    /// Заявление 
    /// </summary>
    public class ApplicationVM
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid ApplicationId { get; set; } = Guid.NewGuid();

        public int Status { get; set; }
        public string? ApplyNumber { get; set; }
        /// <summary>
        /// Данни 
        /// </summary>
        public List<StepItemVM> ApplicationItems { get; set; } = new();

        public ApplicantVM? GetApplicant()
        {
            return ApplicationItems.Where(x => x.Data is ApplicantVM)
                                   .Select(x => x.Data as ApplicantVM)
                                   .FirstOrDefault();
        }
        public ForeignerVM? GetForeigner()
        {
            return ApplicationItems.Where(x => x.Data is ForeignerVM)
                                   .Select(x => x.Data as ForeignerVM)
                                   .FirstOrDefault();
        }
        public ForeignerSmallListVM? GetForeignerSmallList()
        {
            return ApplicationItems.Where(x => x.Data is ForeignerSmallListVM)
                                   .Select(x => x.Data as ForeignerSmallListVM)
                                   .FirstOrDefault();
        }
        public ApplicationTypeVM? GetApplicationType()
        {
            return ApplicationItems.Where(x => x.Data is ApplicationTypeVM)
                                   .Select(x => x.Data as ApplicationTypeVM)
                                   .FirstOrDefault();
        }

        public ApplicationInfoVM? GetApplicationInfo()
        {
            return ApplicationItems.Where(x => x.Data is ApplicationInfoVM)
                                   .Select(x => x.Data as ApplicationInfoVM)
                                   .FirstOrDefault();
        }
        public EmployerVM? GetEmployer()
        {
            return ApplicationItems.Where(x => x.Data is EmployerVM)
                                   .Select(x => x.Data as EmployerVM)
                                   .FirstOrDefault();
        }
        public EmploymentVM? GetEmployment()
        {
            return ApplicationItems.Where(x => x.Data is EmploymentVM)
                                   .Select(x => x.Data as EmploymentVM)
                                   .FirstOrDefault();
        }
        public DocumentsVM? GetDocuments()
        {
            return ApplicationItems.Where(x => x.Data is DocumentsVM)
                                   .Select(x => x.Data as DocumentsVM)
                                   .FirstOrDefault();
        }

        public void SetData<T>(T data) where T : class
        {
            var item = ApplicationItems.Where(x => x.Data is T)
                                       .FirstOrDefault();
            if (item != null)
            {
                item.Data = data;
            }
        }

        public string? GetApplicationTypeCode()
        {
            return ApplicationItems.Where(x => x.Data is ApplicationTypeVM)
                                   .Select(x => x.Data as ApplicationTypeVM)
                                   .FirstOrDefault()?.ApplicationTypeCode;
        }
    }
}
