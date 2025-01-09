using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using sqlsugar=SqlSugar;
//配置代码
namespace OSDesign.config
{

    class DB
    {
        public sqlsugar.SqlSugarClient getInstance()
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            string resultString = Regex.Match(path, @".*(?=bin\\Debug)").Value;
            //string sqlPath = "Data Source=" + resultString + "\\dataProfile\\schedule.db";//数据库SQLite配置文件
            string sqlPath = "Data Source=" + resultString + "\\DB\\OSDesign.db";//数据库SQLite配置文件
            sqlsugar.ConnectionConfig connectionConfig = new sqlsugar.ConnectionConfig();
            connectionConfig.DbType = sqlsugar.DbType.Sqlite;
            connectionConfig.ConnectionString = @sqlPath;
            connectionConfig.IsAutoCloseConnection = true;
            sqlsugar.SqlSugarClient sqliteDB = new sqlsugar.SqlSugarClient(connectionConfig);
            return sqliteDB;
        }
    }
}
