using c_lan.Models;
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
        public async Task<List<ConnectionProfile>> LoadAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (!File.Exists(_configurationFilePath))
            {
                return new List<ConnectionProfile>();
            }
            try
            {
                string json = await File.ReadAllTextAsync(_configurationFilePath, token);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<ConnectionProfile>();
                }

                List<ConnectionProfile>? profiles = JsonSerializer.Deserialize<List<ConnectionProfile>>(json, _jsonOptions);

                if(profiles == null)
                {
                    return new List<ConnectionProfile>();
                }

                return profiles;
            }
            catch(JsonException js)
            {
                throw new InvalidOperationException("连接配置文件损坏，Json格式错误" , js);
            }

            catch(OperationCanceledException)
            {
                throw;
            }

            catch(IOException io)
            {
                throw new IOException("读取配置文件失败", io);
            }
        }
    }
}
