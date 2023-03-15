using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MyLib
{
    public class MySqlOpration
    {
        private MySqlConnection connection;

        private Dictionary<string, string> connectItem;

        private StringBuilder connectStrBuilder;

        private string connectStr;
        public MySqlConnection Connection { get => connection; set => connection = value; }
        public Dictionary<string, string> ConnectItem { get => connectItem; set => connectItem = value; }
        public StringBuilder ConnectStrBuilder { get => connectStrBuilder; set => connectStrBuilder = value; }
        public string ConnectStr { get => connectStr; set => connectStr = value; }

        public void Init(string connStr = "")
        {
            InitEnv(connStr);
            InitConnection(connStr);
        }

        private void InitEnv(string connStr = "")
        {
            if (connStr == "" || connStr == null)
            {
                connectItem = new Dictionary<string, string>();
                ConnectStrBuilder = new StringBuilder();

                MyIni ini = new MyIni(Directory.GetCurrentDirectory() + "\\ini\\config.ini");

                if (ini.ExistINIFile())
                {
                    List<string> keys = ini.ReadKeys("tool");
                    List<string> values = ini.ReadValues("tool");
                    foreach (string key in keys)
                    {
                        connectItem.Add(key, values[keys.IndexOf(key)]);
                        ConnectStrBuilder.Append(key);
                        ConnectStrBuilder.Append("=");
                        ConnectStrBuilder.Append(values[keys.IndexOf(key)]);
                        ConnectStrBuilder.Append(";");
                    }
                }
            }
        }

        /// <summary> 
        /// 创建数据库连接对象
        /// </summary> 
        /// <returns></returns> 
        private void InitConnection(string connStr = "")
        {
            if (connStr == "" || connStr == null)
            {
                connStr = ConnectStrBuilder.ToString();
            }
            Connection = new MySqlConnection(connStr);
        }

        /// <summary> 
        /// 开启数据链接
        /// </summary> 
        /// <returns></returns> 
        public bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                //错误信息
                Console.WriteLine("连接数据库失败！\n" + ex.Message);
                return false;
            }
        }

        /// <summary> 
        /// 关闭数据链接
        /// </summary> 
        /// <returns></returns> 
        public bool CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
            return true;
        }

        /// <summary> 
        /// 获取一个有效的数据库连接对象 
        /// </summary> 
        /// <returns></returns> 
        public MySqlConnection GetConnection(string connSting)
        {
            MySqlConnection Connection = new MySqlConnection(connSting);
            return Connection;
        }

        /// <summary> 
        /// 使用本实例的链接执行非查询语句
        /// </summary> 
        /// <returns></returns> 
        public void ExecuteNonQuery(string connSting)
        {
            MySqlCommand command = new MySqlCommand(connSting, Connection);
            int count = command.ExecuteNonQuery();
        }

        /// <summary> 
        /// 给定连接的数据库用假设参数执行一个sql命令（不返回数据集） 
        /// </summary> 
        /// <param name="connectionString">一个有效的连接字符串</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>执行命令所影响的行数</returns> 
        public int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {

            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }


        /// <summary> 
        /// 用现有的数据库连接执行一个sql命令（不返回数据集） 
        /// </summary> 
        /// <param name="connection">一个现有的数据库连接</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>执行命令所影响的行数</returns> 
        public int ExecuteNonQuery(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary> 
        ///使用现有的SQL事务执行一个sql命令（不返回数据集） 
        /// </summary> 
        /// <remarks> 
        ///举例: 
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="trans">一个现有的事务</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>执行命令所影响的行数</returns> 
        public int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary> 
        /// 用执行的数据库连接执行一个返回数据集的sql命令 
        /// </summary> 
        /// <remarks> 
        /// 举例: 
        /// MySqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connectionString">一个有效的连接字符串</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>包含结果的读取器</returns> 
        public MySqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary> 
        /// 返回DataSet 
        /// </summary> 
        /// <param name="connectionString">一个有效的连接字符串</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns></returns> 
        public DataSet GetDataSet(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();

                adapter.Fill(ds);
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 返回一个数据表 
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        public DataTable GetDataTable(string cmdText, CommandType cmdType = CommandType.Text, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, this.Connection, null, cmdType, cmdText, commandParameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable ds = new DataTable();

                adapter.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据表 
        /// </summary>
        ///<param name="connectionString">一个有效的连接字符串</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        public DataTable GetDataTable(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataTable ds = new DataTable();

                adapter.Fill(ds);
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary> 
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列 
        /// </summary> 
        /// <remarks> 
        ///例如: 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        ///<param name="connectionString">一个有效的连接字符串</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns> 
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 返回插入值ID
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public object ExecuteNonExist(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteNonQuery();

                return cmd.LastInsertedId;
            }
        }

        /// <summary> 
        /// 用指定的数据库连接执行一个命令并返回一个数据集的第一列 
        /// </summary> 
        /// <remarks> 
        /// 例如: 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个存在的数据库连接</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns> 
        public object ExecuteScalar(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {

            MySqlCommand cmd = new MySqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary> 
        /// 准备执行一个命令 
        /// </summary> 
        /// <param name="cmd">sql命令</param> 
        /// <param name="conn">OleDb连接</param> 
        /// <param name="trans">OleDb事务</param> 
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param> 
        /// <param name="cmdText">命令文本,例如:Select * from Products</param> 
        /// <param name="cmdParms">执行命令的参数</param> 
        private void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }

        /// <summary>
        /// 记录使用情况
        /// </summary>
        /// <param name="type">工具类型</param>
        /// <param name="log_time">log开始时间</param>
        /// <param name="sys_time">当前时间</param>
        /// <param name="step">步骤</param>
        public void logToDB(string type, DateTime log_time, DateTime sys_time, string step)
        {
            //获取IP hostName的
            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());
            //获取IPV4的地址的
            if (ipe.AddressList.Length > 1)
            {
                IPAddress ipaddr = ipe.AddressList[1];
                TimeSpan cost_time = sys_time - log_time;
                string sql = string.Format("insert into useinfo_log(id,type,log_time,sys_time,cost_time,step,ip) values(" + "{0},'{1}','{2}','{3}','{4}','{5}','{6}'" + "); "
                    , 0, type, log_time.ToString("yy-MM-dd HH:mm:ss"), sys_time.ToString("yy-MM-dd HH:mm:ss"), cost_time.TotalSeconds, step, ipaddr.ToString());
                ExecuteNonQuery(sql);
            }

        }
    }
}
