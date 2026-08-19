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
        Task<ConnectionResult> TestConnectionAsync(ConnectionProfile profile,CancellationToken cancellationToken);

        Task<List<ConnectionProfile>> ReadAllConfigurationsAsync(CancellationToken cancellationToken);

        Task<SaveConfigurationResult> SaveConnectionConfigurationAsync(ConnectionProfile profile, CancellationToken token);

        Task<SaveConfigurationResult> DeleteConnectionConfigurationAsync(string connectionName, CancellationToken token);
    }
}
