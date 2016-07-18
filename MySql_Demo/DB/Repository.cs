using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySql_Demo.DB
{
    /// <summary>
    /// 仓储
    /// </summary>
    public class Repository : IRepository
    {
        public MySqlHelper db;
        public Repository(string connectionString)
        {
            db = new MySqlHelper(connectionString);
        }

        #region Insert/Update/Delete
        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public int Insert(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                string tableName = dt.TableName;
                List<MySqlParameter> param = new List<MySqlParameter>();
                string sql = "insert into " + tableName + "(";
                string cols = "";
                string vals = "";
                DataRow dr = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                {
                    string key = col.ColumnName;

                    if (dr[key] != DBNull.Value)
                    {
                        cols += key + ",";
                        vals += "@" + key + ",";
                        param.Add(new MySqlParameter(key, dr[key]));
                    }
                }
                cols = cols.Substring(0, cols.Length - 1);
                vals = vals.Substring(0, vals.Length - 1);
                sql += cols + ")values(" + vals + ") select @@identity";
                Object objValue = db.ExecuteNonQuery(sql, param.ToArray());
                if (objValue == null)
                    return 0;
                else
                    return Convert.ToInt32(objValue);
            }
            else { return 0; }
        }
        /// <summary>
        /// 修改记录
        /// </summary>
        public int Update(DataTable dt, string where)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("update {0} set ", dt.TableName);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i != 0)
                {
                    sbSql.Append(",");
                }
                sbSql.AppendFormat("[{0}]='{1}'", dt.Columns[i].ColumnName, dt.Rows[0][i].ToString());
            }
            sbSql.AppendFormat(" where {0}", where);
            return db.ExecuteNonQuery(CommandType.Text, sbSql.ToString());
        }
       
        /// <summary>
        /// 删除记录
        /// </summary>
        public int Delete(string tableName)
        {
            return Delete(tableName, "");
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        public int Delete(string tableName, string where)
        {
            StringBuilder sbSql = new StringBuilder();
            if (where == "")
                sbSql.AppendFormat("delete from {0}", tableName);
            else
                sbSql.AppendFormat("delete from {0} where {1}", tableName, where);
            return db.ExecuteNonQuery(CommandType.Text, sbSql.ToString());
        }
        /// <summary>
        /// 删除记录
        /// </summary>
        public int Delete(string tableName, string columnName, object value)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("delete from {0} where {1}=@{1}", tableName, columnName);
            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter(columnName,value)
            };
            return db.ExecuteNonQuery(CommandType.Text, sbSql.ToString(), param);
        }
        #endregion

        #region DataTable
        /// <summary>
        /// 获取DataTable表数据
        /// </summary>
        public DataTable GetDataTable(string tableName)
        {
            DataTable dt = GetDataTable(tableName, "*", "");
            dt.TableName = tableName;
            return dt;
        }
        /// <summary>
        /// 获取DataTable表数据
        /// </summary>
        public DataTable GetDataTable(string tableName, string fieldName)
        {
            return GetDataTable(tableName, fieldName, "");
        }
        /// <summary>
        /// 获取DataTable数据集
        /// </summary>
        /// <param name="fieldName">数据字段，如：string fields="ID,Name,Sex";如为"*",则为所有字段</param>
        /// <param name="where">条件，如：string fields="ID=1"</param>
        public DataTable GetDataTable(string tableName, string fieldName, string where)
        {
            StringBuilder sbSql = new StringBuilder();
            if (where == "")
                sbSql.AppendFormat("select {0} from {1}", fieldName, tableName);
            else
                sbSql.AppendFormat("select {0} from {1} where {2}", fieldName, tableName, where);
            return db.ExecuteDataTable(CommandType.Text, sbSql.ToString());
        }
        /// <summary>
        /// 获取指定条件的DataTable数据集
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="fieldName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public DataTable GetDataByKey(string tableName, string fieldName, object value)
        {
            string sql = "select * from " + tableName + " where " + fieldName + "=@" + fieldName;
            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter(fieldName,value)
            };
            DataTable dt = db.ExecuteDataTable(CommandType.Text, sql, param);
            dt.TableName = tableName;
            return dt;
        }
        /// <summary>
        /// 获取指定条件的DataTable数据集
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="hs"></param>
        /// <returns></returns>
        public DataTable GetDataByWhere(string tablename, Hashtable hs)
        {
            string sql = "select * from " + tablename + " where 1=1";
            List<MySqlParameter> param = new List<MySqlParameter>();
            foreach (string key in hs.Keys)
            {
                sql += string.Format(" and {0}=@{0}", key);
                param.Add(new MySqlParameter(key, hs[key]));
            }
            DataTable dt = db.ExecuteDataTable(CommandType.Text, sql, param.ToArray());
            dt.TableName = tablename;
            return dt;
        }
        /// <summary>
        /// 获取排序数据
        /// </summary>
        public DataTable GetSortDataByKey(string tablename, string fieldName, bool asc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from {0} order by [{1}]  {2}", tablename, fieldName, asc ? "asc" : "desc");
            DataTable dt = db.ExecuteDataTable(CommandType.Text, sb.ToString());
            dt.TableName = tablename;
            return dt;
        }
        #endregion

        #region 执行Sql语句
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="cType">执行类型</param>
        /// <param name="sql">sql语句</param>
        /// <returns>影响记录数</returns>
        public int ExecteSql( string sql,CommandType commandType = CommandType.Text)
        {
            return db.ExecuteNonQuery(commandType, sql);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="cType">执行类型</param>
        /// <param name="sql">sql语句</param>
        /// <returns>影响记录数</returns>
        public int ExecteSql(string sql, MySqlParameter[] param, CommandType commandType = CommandType.Text)
        {
            return db.ExecuteNonQuery(commandType, sql, param);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="cType">执行类型</param>
        /// <param name="sql">sql语句</param>
        /// <returns>结果集DataSet</returns>
        public DataSet ExecteSqlDataSet( string sql,CommandType commandType = CommandType.Text)
        {
            return db.ExecuteDataSet(commandType, sql);
        }
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="cType">执行类型</param>
        /// <param name="sql">sql语句</param>
        /// <returns>表数据DataTable</returns>
        public DataTable ExecteSqlGetDataTable(string sql,CommandType commandType = CommandType.Text)
        {
            return db.ExecuteDataTable(commandType, sql);
        }
        #endregion

    }
}
