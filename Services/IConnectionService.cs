using c_lan.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace c_lan.Services
{
    public interface IConnectionService
    {
        Task<bool> TestConnectionAsync(
            ConnectionProfile profile,
            CancellationToken cancellationToken);

/*        Task<List<ConnectionProfile>> ReadallConfigurationAsync(CancellationToken cancellationToken);

        Task<bool> SaveConnectionConfigurationAsync(
            ConnectionProfile profile, bool keeporNot, CancellationToken token);

        Task<bool> DeleteConnectionConfigurationAsync(
            int configid, CancellationToken token);*/
    }
}
