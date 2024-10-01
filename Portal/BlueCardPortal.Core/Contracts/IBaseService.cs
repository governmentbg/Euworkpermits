using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlueCardPortal.Core.Contracts
{
    /// <summary>
    /// Базова услуга за достъп до базата данни
    /// </summary>
    public interface IBaseService
    {
        /// <summary>
        /// Извлича данни за един обект по идентификатор
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetById<T>(object id) where T : class;
       
    }
}
