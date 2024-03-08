using System.Threading;
using System.Threading.Tasks;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman.Services
{
    public interface IUserLoginService
    {
        string GetUserLoginByPersonnelNumber(string personnelNumber);

        Task<string> GetUserLoginByPersonnelNumberAsync(string personnelNumber,
            CancellationToken cancellationToken = default);
    }
}