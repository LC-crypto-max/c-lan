using c_lan.Models;
using Microsoft.VisualBasic;
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
            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            _configurationDirectoryPath = Path.Combine(appDataPath, "c-lan");

            _configurationFilePath = Path.Combine(_configurationDirectoryPath, "connections.json");
            //这里是JSON的相关配置
            _jsonOptions = new JsonSerializerOptions{WriteIndented = true,PropertyNameCaseInsensitive = true};
        }
        public async Task<List<ConnectionProfile>> LoadAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            //验证文件是否存在
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
                //这是使用了JSON反序列化
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

        public async Task SaveAsync(List<ConnectionProfile> profiles, CancellationToken token)
        {
            token.ThrowIfCancellationRequested ();

            if (profiles == null)
            {
                //判断连接配置存在
                throw new ArgumentNullException(nameof(profiles));
            }
                Directory.CreateDirectory(_configurationDirectoryPath);

            try
            {
                //JSON序列化
                string json = JsonSerializer.Serialize(profiles, _jsonOptions);
                //等待异步写入数据
                await File.WriteAllTextAsync(_configurationFilePath,json,Encoding.UTF8, token);

                
            }
            catch(OperationCanceledException)
            {
                throw;
            }

            catch(JsonException js)
            {
                throw new InvalidOperationException("存在无法序列化的数据", js);
            }

            catch(IOException io)
            {
                throw new IOException("保存配置文件失败", io);
            }
        }
    }
}
