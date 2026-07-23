using c_lan.Models;
using c_lan.Services;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;

namespace c_lan
{
    public partial class Form1 : Form
    {
        private ConnectionService _connectionService;
        private ConnectionProfile _connectionProfile;
        private CancellationTokenSource? _cancellationTokenSource;

        public Form1()
        {
            InitializeComponent();

            _connectionService = new ConnectionService();

        }

        private CancellationTokenSource CreatenewCancellToken()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();

                _cancellationTokenSource.Dispose();
            }

            _cancellationTokenSource = new CancellationTokenSource();

            return _cancellationTokenSource;
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            CancellationTokenSource cancellation = CreatenewCancellToken();

            try
            {
                TestButton.Enabled = false;

                TestButton.Text = "测试中...";

                if (String.IsNullOrWhiteSpace(ConnectionnameText.Text))
                {
                    MessageBox.Show("请输入连接名称");
                    return;
                }

                if (String.IsNullOrWhiteSpace(HostText.Text))
                {
                    MessageBox.Show("请输入主机地址");
                    return;
                }
                if (String.IsNullOrWhiteSpace(UserText.Text))
                {
                    MessageBox.Show("请输入用户名");
                    return;
                }
                if (String.IsNullOrWhiteSpace(PasswordText.Text))
                {
                    MessageBox.Show("请输入密码");
                    return;
                }

                _connectionProfile = new ConnectionProfile()

                {
                    ConnectionName = ConnectionnameText.Text,
                    Host = HostText.Text,
                    Port = uint.TryParse(PortText.Text, out uint parsedPort) ? parsedPort : 0,
                    UserName = UserText.Text,
                    Password = PasswordText.Text,
                    ConnectionTimeout = 10
                };

                Debug.WriteLine("开始测试连接");

                bool success = await _connectionService.TestConnectionAsync(_connectionProfile, cancellation.Token);

                Debug.WriteLine("测试完成");

                if (success)
                {
                    MessageBox.Show("连接成功");
                }
                else
                {
                    MessageBox.Show("连接失败");
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("遇到未知错误");

                MessageBox.Show(ex.Message, "是本次错误原因", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally{
                TestButton.Enabled = true;
                TestButton.Text = "测试连接";

                if(_cancellationTokenSource == cancellation){
                    _cancellationTokenSource.Dispose();
                }

            }
        }
        
    }
}
