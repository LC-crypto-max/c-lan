using c_lan.Models;

namespace c_lan.Services
{
    //Schema Service负责组织数据库对象浏览流程，不保存MySQL专用SQL。
    public interface ISchemaService
    {
        Task<List<string>> GetDatabasesAsync(ConnectionProfile profile, CancellationToken token);

        Task<List<DatabaseObjectInfo>> GetObjectsAsync(ConnectionProfile profile, string databaseName, CancellationToken token);

        Task<List<ColumnInfo>> GetColumnsAsync(ConnectionProfile profile, string databaseName, string objectName,CancellationToken token);

        Task<QueryResult> PreviewAsync(ConnectionProfile profile, string databaseName, string objectName,int maxRows, CancellationToken token);
    }
}
