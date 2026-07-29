using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace c_lan.Configuration
{
    public class ConnectionProfileStore
    {
        private readonly String _configurationDirectoryPath;
        private readonly String _configurationFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConnectionProfileStore()
        {
            String appDataPath = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData);
            _configurationDirectoryPath = Path.Combine(appDataPath, "c-lan");

            _configurationFilePath = Path.Combine(_configurationDirectoryPath, "connections.json");

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }
        public async List<> LoadAsync(CancellationToken token)
        {

        }
    }
}
