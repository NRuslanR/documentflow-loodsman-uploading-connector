using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UMP.DocumentFlow.Dtos.Converters.Loodsman.Services
{
    public class DomainUserLoginService : IUserLoginService
    {
        public async Task<string> GetUserLoginByPersonnelNumberAsync(string personnelNumber,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetUserLoginByPersonnelNumber(personnelNumber), cancellationToken);
        }

        public string GetUserLoginByPersonnelNumber(string personnelNumber)
        {
            return Environment.UserDomainName + Path.DirectorySeparatorChar + personnelNumber;
        }
    }
}