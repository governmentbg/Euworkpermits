using BlueCardPortal.Core.Contracts;
using IO.SignTools.Contracts;
using IO.SignTools.Models;

namespace BlueCardPortal.Core.Services
{
    public class SignerService(IIOSignToolsService signToolsService) : ISignerService
    {
        /// <summary>
        /// Подпечатва файл detached
        /// </summary>
        /// <param name="data">Файл за подпечатване</param>
        /// <param name="tsOptions">Настройки на времевата марка</param>
        /// <param name="filePath">Път до печата</param>
        /// <param name="password">Парола на печата (optional)</param>
        /// <returns></returns>
        public async Task<(string signature, byte[] tsr, DateTime timestamp)> StampIt(byte[] data, TimestampClientOptions tsOptions, string filePath, string? password = null)
        {
            return await signToolsService.StampDetachedWithTS(data, tsOptions, filePath, password);
        }
    }
}
