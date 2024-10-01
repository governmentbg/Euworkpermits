using BlueCardPortal.Infrastructure.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Extensions
{
    public static class NomenclatureExtensions
    {
        
        /// <summary>
        /// Връща датата ако не е null с час 23:59, независимо от часа в нея
        /// </summary>
        /// <param name="model">час</param>
        /// <returns></returns>
        public static DateTime? ForceEndDate(this DateTime? model)
        {
            if (model.HasValue)
            {
                return model.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            return model;
        }

        /// <summary>
        /// Връща датата ако не е null с час 23:59, независимо от часа в нея
        /// </summary>
        /// <param name="model">час</param>
        /// <returns></returns>
        public static DateTime ForceEndDate(this DateTime model)
        {
            return model.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        /// <summary>
        /// Добавя на датата 0:00 часа
        /// </summary>
        /// <param name="model">час</param>
        /// <returns></returns>
        public static DateTime ForceStartDate(this DateTime model)
        {
            return model.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
        }

        /// <summary>
        /// Добавя на дата 0:00 часа
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static DateTime? ForceStartDate(this DateTime? model)
        {
            if (model.HasValue)
            {
                return model.Value.ForceStartDate();
            }
            return model;
        }

        /// <summary>
        /// Конвертитане на локално време към UTC
        /// </summary>
        /// <param name="model">Дата</param>
        /// <returns></returns>
        public static DateTime ConvertToUtc(this DateTime model)
        {
            return TimeZoneInfo.ConvertTimeToUtc(model);
        }

        /// <summary>
        /// Конвертитане на локално време към UTC
        /// </summary>
        /// <param name="model">Дата</param>
        /// <returns></returns>
        public static DateTime? ConvertToUtc(this DateTime? model)
        {
            if (model.HasValue)
            {
                DateTime dateTime = model.Value;
                return TimeZoneInfo.ConvertTimeToUtc(dateTime);
            }
            else
                return model;
        }

        /// <summary>
        /// Конвертира стойност към нулл ако е по-малка от 1ца
        /// </summary>
        /// <param name="model">Long</param>
        /// <returns></returns>
        public static long? ToLongOrNull(this long? model)
        {
            if (model.HasValue)
            {
                if (model <= 0)
                    return null;
                else
                    return model;
            }
            else
                return null;
        }

        /// <summary>
        /// Конвертира стрингова стойност с гуид към стринг
        /// </summary>
        /// <param name="model">Гуид в стрингов тип</param>
        /// <returns></returns>
        public static string? ToGuidStringToString(this string? model)
        {
            if (!string.IsNullOrEmpty(model) && model != "00000000-0000-0000-0000-000000000000" && model != "0")
                return model;
            else
                return null;
        }

        /// <summary>
        /// Конвертиране на дата в стринг
        /// </summary>
        /// <param name="model">Дата</param>
        /// <returns></returns>
        public static string ConvertDateTimeToString(this DateTime? model)
        {
            if (model.HasValue)
            {
                DateTime dateTime = model.Value;
                return dateTime.ToString("dd.MM.yyyy");
            }
            else
                return string.Empty;
        }
        public static string GetIOReqClass(this Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata model)
        {
            try
            {
                if (model.ContainerType != null)
                {
                    var Member = model.ContainerType.GetMember(model.PropertyName);
                    var reqTypes = new[] {
                        typeof(RequiredAttribute),
                        typeof(RangeAttribute),
                        typeof(IORequiredAttribute)
                    };
                    var hasIOreq = Member[0].CustomAttributes.Any(a => reqTypes.Contains(a.AttributeType));
                    if (hasIOreq)
                    {
                        return "io-req";
                    }
                }
            }
            catch { }
            try
            {
                if (model.ContainerType != null)
                {
                    var Member = model.ContainerType.GetMember(model.PropertyName);
                    var reqTypes = new[] {
                        typeof(IOconditionallyRequiredAttribute)
                    };
                    var hasIOreq = Member[0].CustomAttributes.Any(a => reqTypes.Contains(a.AttributeType));
                    if (hasIOreq)
                    {
                        return "io-conditionally-req";
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public static string CombineTemplatePrefix(this string prefix, string add)
        {
            return (string.IsNullOrEmpty(prefix) ? string.Empty : prefix + ".") + add;
        }
        
    }
}
