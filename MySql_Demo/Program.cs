using MySql.Data.MySqlClient;
using MySql_Demo.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MySql_Demo
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Repository db = new Repository(System.Configuration.ConfigurationManager.AppSettings["MySql"].ToString());
            /*
            DataTable dt = db.GetDataTable("T_User");
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    Console.Write(dr[c.ColumnName]);
                    Console.Write("\t");
                }
                Console.Write("\n");
            }
            Console.Read();
            */

            /*
            string sql = @"INSERT INTO `T_User` ( `UserName`, `Password`, `RealName`, `RoleID`) 
                            VALUES (@UserName, @Password, @RealName, @RoleID);
                            select @@identity;";

            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("UserName","001"),
                new MySqlParameter("Password","123456"),
                new MySqlParameter("RealName","张三"),
                new MySqlParameter("RoleID",1)
            };
            int res = db.ExecteSql(sql, param);
            */
            string sql = @"INSERT INTO `T_User` ( 
		                    `UserName`, 
		                    `Password`, 
		                    `RealName`
	                    ) 
	                    VALUES (
		                    @IN_UserName,
		                    @IN_Password,
		                    @IN_RealName
	                    ); ";

            MySqlParameter[] param = new MySqlParameter[]{
                new MySqlParameter("IN_UserName","001"),
                new MySqlParameter("IN_Password","123456"),
                new MySqlParameter("IN_RealName","张三")
            };
            //int res = db.ExecteSql("SP_UserAdd", param, CommandType.StoredProcedure);
            int res = db.ExecteSql(sql, param, CommandType.Text);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
