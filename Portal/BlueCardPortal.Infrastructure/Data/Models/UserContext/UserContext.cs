using BlueCardPortal.Infrastructure.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Security.Claims;
using static BlueCardPortal.Infrastructure.Constants.CustomClaimType;

namespace BlueCardPortal.Infrastructure.Data.Models.UserContext
{
    /// <summary>
    /// Информация за потребител
    /// </summary>
    public class UserContext : IUserContext
    {
        private readonly ClaimsPrincipal User;
        private readonly HttpContext httpContext;
        private readonly IStringLocalizer localizer;
        public UserContext(
            IHttpContextAccessor _ca,
            IStringLocalizer _localizer)
        {
            httpContext = _ca?.HttpContext ?? throw new ArgumentException("HttpContext unavailable");
            User = _ca.HttpContext.User;
            localizer = _localizer;
        }

        /// <summary>
        /// Идентификатор на потребител
        /// </summary>
        public Guid Id { 
            get 
            { 
                Guid guid = Guid.Empty;
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(id, out guid))
                {
                    return guid;
                }

                return guid;
            } 
        }

        /// <summary>
        /// Име на потребител
        /// </summary>
        public string Name
        {
            get
            {
                string name = string.Empty;
                var value = User.FindFirstValue(EAuth.PersonName);

                if (value != null)
                {
                    name = value;
                }

                return name;
            }
        }

        /// <summary>
        /// Персонален идентификатор
        /// </summary>
        public string Pid
        {
            get
            {
                string pid = string.Empty;
                var value = User.FindFirstValue(EAuth.PersonIdentifier);

                if (value != null)
                {
                    pid = value;
                }

                return pid;
            }
        }

        /// <summary>
        /// Тип на идентификатора
        /// </summary>
        public string PidType
        {
            get
            {
                string pidType = string.Empty;
                var value = User.FindFirstValue(EAuth.IdentifierType);

                if (value != null)
                {
                    pidType = value;
                }

                return pidType;
            }
        }

        /// <summary>
        /// Имейл на потребител
        /// </summary>
        public string? Email
        {
            get
            {
                string? email = null;
                var value = User.FindFirstValue(EAuth.Email);

                if (value != null)
                {
                    email = value;
                }

                return email;
            }
        }

        /// <summary>
        /// ЕИК на организацията
        /// </summary>
        public string? Eik
        {
            get
            {
                string? eik = null;
                var value = User.FindFirstValue(EAuth.Eik);

                if (value != null)
                {
                    eik = value;
                }

                return eik;
            }
        }

        /// <summary>
        /// Име на организацията
        /// </summary>
        public string? OrganizationName
        {
            get
            {
                string? organizationName = null;
                var value = User.FindFirstValue(EAuth.OrganizationName);

                if (value != null)
                {
                    organizationName = value;
                }

                return organizationName;
            }
        }

        /// <summary>
        /// Проверка дали потребителят е аутентикиран
        /// </summary>
        public bool IsAuthenticated => User.Identity?.IsAuthenticated ?? false;
        public string LoginLabel()
        {
            var pidType = localizer[PidType].ToString();
            return $"{pidType}: {Pid}" + (string.IsNullOrEmpty(Eik) ? string.Empty : $" {localizer["EIK"]} : {Eik}");
        }
    }
}

