using System.Collections.Generic;
using System.Threading.Tasks;
using Logs.Core.Domain;

namespace Logs.Core.Abstractions
{
    public interface IMongoService
    {
        Task Create(LogMessageEntity log);
        Task<LogMessageEntity> GetAsync(string id);
        Task<IEnumerable<LogMessageEntity>> GetLogsAsync(string? type, string? iniciator);
        Task DeleteRangeAsync();
        Task<bool> DeleteSelectedAsync(List<string> ids);
        Task<bool> DeleteAsync(string id);
    }
}