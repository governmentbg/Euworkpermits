
using BlueCardPortal.Core.Contracts;
using BlueCardPortal.Infrastructure.Contracts;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Core.Services
{
    public class BaseService : IBaseService
    {
        protected IRepository repo;
        protected IUserContext userContext;
        protected ILogger<BaseService> logger;



        public async Task<T> GetById<T>(object id) where T : class
        {
            return await repo.GetByIdAsync<T>(id);
        }

        protected async Task<GridResponseModel> CalcGridResponseModelAsync<T>(IQueryable<T> query, int? inPage, long? inPageSize)
        {
            int page = inPage ?? 1;
            page = page >= 1 ? page : 1;
            int pageSize = (int)(inPageSize ?? 10);
            pageSize = pageSize < 10 ? 10 : pageSize;
            var count = await query.CountAsync();
            int totalPages = count / pageSize;
            if (totalPages * pageSize < count)
                totalPages += 1;
            return new GridResponseModel
            {
                data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                page = page,
                total_pages = totalPages,
                size = pageSize,
                total_rows = count,
            };
        }

        protected GridResponseModel CalcGridResponseModel<T>(IQueryable<T> query, int? inPage, long? inPageSize)
        {
            int page = inPage ?? 1;
            page = page >= 1 ? page : 1;
            int pageSize = (int)(inPageSize ?? 10);
            pageSize = pageSize < 10 ? 10 : pageSize;
            var count = query.Count();
            int totalPages = count / pageSize;
            if (totalPages * pageSize < count)
                totalPages += 1;
            return new GridResponseModel
            {
                data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                page = page,
                total_pages = totalPages,
                size = pageSize,
                total_rows = count,
            };
        }
        protected DateTime dtNow = DateTime.Now;

    }

}
