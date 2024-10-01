using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlueCardPortal.Core.Extensions
{
    public static class GridViewExtensions
    {
        public static async Task<GridResponseModel> BuildResponseAsync<T>(this GridRequestModel request, IQueryable<T> source) where T : class
        {
            var response = new GridResponseModel()
            {
                page = request.page,
                size = request.size,
                data = new { }
            };

            if (source != null && request.size > 0)
            {
                response.total_rows = await source.CountAsync();
                response.total_pages = (int)Math.Ceiling((decimal)response.total_rows / request.size);
                var dataPage = await source
                    .Skip((request.page - 1) * request.size)
                    .Take(request.size)
                    .ToListAsync();

                response.data = dataPage;
            }

            return response;
        }

        public static async Task<IActionResult> GetResponseAsync<T>(this GridRequestModel request, IQueryable<T> source) where T : class
        {
            var response = await BuildResponseAsync(request, source);

            return new JsonResult(response);
        }

        public static T? GetData<T>(this GridRequestModel request)
        {
            if (request.data == null)
            {
                return (T?)Activator.CreateInstance(typeof(T), false);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(request.data, new IsoDateTimeConverter { DateTimeFormat = FormattingConstant.NormalDateFormat });
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
