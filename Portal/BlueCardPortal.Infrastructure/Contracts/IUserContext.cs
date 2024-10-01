using System;
using System.Collections.Generic;
using System.Text;
using static BlueCardPortal.Infrastructure.Constants.CustomClaimType;

namespace BlueCardPortal.Infrastructure.Contracts
{
    /// <summary>
    /// Текущ контекст на изпълнение на операциите
    /// </summary>
    public interface IUserContext
    {
        /// <summary>
        /// Идентификатор на потребител
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Електронна поща на потребител
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Имена на потребител
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Персонален идентификатор
        /// </summary>
        string Pid { get; }
        /// <summary>
        /// Тип на идентификатора
        /// </summary>
        string PidType { get; }
        /// <summary>
        /// ЕИК на организацията
        /// </summary>
        string? Eik { get; }
        /// <summary>
        /// Име на организацията
        /// </summary>
        string? OrganizationName { get; }
        /// <summary>
        /// Проверка дали потребителят е аутентикиран
        /// </summary>
        bool IsAuthenticated { get; }
    }
}
