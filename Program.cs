using c_lan.Configuration;
using c_lan.Data;
using c_lan.Services;

namespace c_lan
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //进行组装,职责拆分
            ApplicationConfiguration.Initialize();
            ConnectionProfileStore store= new ConnectionProfileStore();
            DatabaseProviderFactory factory= new DatabaseProviderFactory();
            IConnectionService connectionService = new ConnectionService(store,factory);
            //对象浏览也通过Service进入Provider，Form不直接创建MysqlProvider。
            ISchemaService schemaService = new SchemaService(factory);

            Form1 form1 = new Form1(connectionService, schemaService);
            Application.Run(form1);
        }
    }
}
