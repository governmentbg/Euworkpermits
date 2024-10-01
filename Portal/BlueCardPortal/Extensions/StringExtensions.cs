using BlueCardPortal.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BlueCardPortal.Extensions
{
    public static class StringExtensions
    {
        public static string BareName<T>() where T : Controller
        {
            return typeof(T).Name.Replace(nameof(Controller), string.Empty);
        }
        public static string Decode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return System.Web.HttpUtility.HtmlDecode(value);
        }
    }
}
