using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Infrastructure.Model.ApplicationList
{
    public class ComplaintListItemVM
    {
        /// <summary>
        /// Номер жалба
        /// </summary>
        public string ComplaintNumber { get; set; } = default!;


        /// <summary>
        /// Номер заявление
        /// </summary>
        public string ApplicationNumber { get; set; } = default!;
        
        /// <summary>
        /// Име чужденец
        /// </summary>
        public string ComplaintName { get; set; } = default!;

        /// <summary>
        /// Статус
        /// </summary>
        
        public string Status { get; set; } = default!;
        /// <summary>
        /// Вид разрешително
        /// </summary>
        
        public string PermitType { get; set; } = default!;

        /// <summary>
        /// Дата на status
        /// </summary>
        public DateTime? StatusDate { get; set; }

        /// <summary>
        /// Дата на подаване
        /// </summary>
        public DateTime? ComplaintDate { get; set; }
    }
}
