using System;
using System.Configuration;
using System.Collections; 
using System.Data;
using System.Data.SqlClient;

namespace MyData
{
    /// <summary>
    /// 通用数据库类MSSQL 
    /// </summary>
    public class MSSQL
    {

        //public static string ConnStr = @"server=192.168.1.2;uid=sa;pwd=sa;database=my_soft;";
        public static string ConnStr = @MyData.Properties.Settings.Default.my_soft_sqlConn;


        //打开数据库链接
        public static SqlConnection Open_Conn(string ConnStr)
        {
            try
            {
                SqlConnection Conn = new SqlConnection(ConnStr + "Connect Timeout=5;");
                Conn.Open();
                return Conn;
            }
            catch
            {
                throw new Exception("数据库链接失败，请检查！");
            }
        }

        //关闭数据库链接
        public static void Close_Conn(SqlConnection Conn)
        {
            try
            {
                if (Conn != null)
                {
                    Conn.Close();
                    Conn.Dispose();
                }
                GC.Collect();
            }
            catch { }
        }

        //运行Sql语句
        public static int Run_SQL(string SQL, string ConnStr)
        {
            try
            {                
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlCommand Cmd = Create_Cmd(SQL, Conn);
                int result_count = Cmd.ExecuteNonQuery();
                Close_Conn(Conn);
                return result_count;
            }
            catch
            {
                //throw new Exception("操作执行失败，请检查！\n\n如有问题，请与技术人员联系，给您带来不便敬请谅解！");
                return -1;
            }
        }

        // 生成Command对象 
        public static SqlCommand Create_Cmd(string SQL, SqlConnection Conn)
        {
            try
            {
                SqlCommand Cmd = new SqlCommand(SQL, Conn);
                return Cmd;
            }
            catch
            {
                return null;
            }
        }

        // 运行Sql语句返回 DataTable
        public static DataTable Get_DataTable(string SQL, string ConnStr, string Table_name)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlDataAdapter Da = new SqlDataAdapter(SQL, Conn);
                DataTable dt = new DataTable(Table_name);
                Da.Fill(dt);
                Close_Conn(Conn);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        // 运行Sql语句返回 SqlDataReader对象
        public static SqlDataReader Get_Reader(string SQL, string ConnStr)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlCommand Cmd = Create_Cmd(SQL, Conn);
                SqlDataReader Dr;
                Dr = Cmd.ExecuteReader(CommandBehavior.Default);
                Close_Conn(Conn);
                return Dr;
            }
            catch
            {
                return null;
            }
        }

        // 运行Sql语句返回 SqlDataAdapter对象 
        public static SqlDataAdapter Get_Adapter(string SQL, string ConnStr)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlDataAdapter Da = new SqlDataAdapter(SQL, Conn);
                return Da;
            }
            catch
            {
                return null;
            }
        }

        // 运行Sql语句,返回DataSet对象
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlDataAdapter Da = new SqlDataAdapter(SQL, Conn);
                Da.Fill(Ds);
                Close_Conn(Conn);
                return Ds;
            }
            catch
            {
                return null;
            }
        }

        // 运行Sql语句,返回DataSet对象
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds, string tablename)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlDataAdapter Da = new SqlDataAdapter(SQL, Conn);
                Da.Fill(Ds, tablename);
                Close_Conn(Conn);
                return Ds;
            }
            catch
            {
                return null;
            }
        }

        // 运行Sql语句,返回DataSet对象，将数据进行了分页
        public static DataSet Get_DataSet(string SQL, string ConnStr, DataSet Ds, int StartIndex, int PageSize, string tablename)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                SqlDataAdapter Da = new SqlDataAdapter(SQL, Conn);
                Da.Fill(Ds, StartIndex, PageSize, tablename);
                Close_Conn(Conn);
                return Ds;
            }
            catch
            {
                return null;
            }
        }

        // 返回Sql语句执行结果的第一行第一列
        public static string Get_Row1_Col1_Value(string SQL, string ConnStr)
        {
            try
            {
                SqlConnection Conn = Open_Conn(ConnStr);
                string result;
                SqlDataReader Dr;
                Dr = Create_Cmd(SQL, Conn).ExecuteReader();
                if (Dr.Read())
                {
                    result = Dr[0].ToString();
                    Dr.Close();
                }
                else
                {
                    result = "";
                    Dr.Close();
                }
                Close_Conn(Conn);
                return result;
            }
            catch
            {
                return null;
            }
        }


        #region 存储过程 相关操作
        /*
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // 存储过程 相关操作
        ///////////////////////////////////////////////////////////////////////////////////////////////////

        // 生成一个存储过程使用的Sqlcommand.
        // procName 存储过程名, prams 存储过程入参数组, 返回 SqlCommand对象
        public static SqlCommand Proc_Create_Cmd(string procName, SqlParameter[] prams, string ConnStr) 
        {
            SqlConnection Conn = Open_Conn(ConnStr);
            SqlCommand Cmd = new SqlCommand(procName, Conn);
            Cmd.CommandType = CommandType.StoredProcedure;
            if (prams != null) 
            {
                foreach (SqlParameter parameter in prams)
                {
                    if(parameter != null)
                    {
                        Cmd.Parameters.Add(parameter);
                    }
                }
            }
            return Cmd;
        }

        // 生成一个存储过程使用的SqlCommand.
        // procName 存储过程名, prams 存储过程入参数组, 返回 SqlCommand对象
        private static SqlCommand Proc_Create_Cmd(string procName, SqlParameter[] prams, string ConnStr, SqlDataReader Dr) 
        {
            SqlCommand Cmd = Proc_Create_Cmd(procName, prams, ConnStr);
            if (prams != null) 
            {
                foreach (SqlParameter parameter in prams)
                Cmd.Parameters.Add(parameter);
            }
            Cmd.Parameters.Add(
            new SqlParameter("ReturnValue", SqlDbType.Int32, 4,
            ParameterDirection.ReturnValue, false, 0, 0,
            string.Empty, DataRowVersion.Default, null));

            return Cmd;
        }

        // 运行存储过程,返回 SqlDataReader对象
        public static void Proc_Get_Reader(string procName, SqlParameter[] prams, string ConnStr, SqlDataReader Dr) 
        {
            SqlCommand Cmd = Proc_Create_Cmd(procName, prams, ConnStr, Dr);
            Dr = Cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            return;
        }

        // 运行存储过程,返回执行结果的第一行第一列
        public static string Proc_Get_Value(string procName, SqlParameter[] prams, string ConnStr) 
        {
            SqlDataReader Dr;
            SqlCommand Cmd = Proc_Create_Cmd(procName, prams, ConnStr);
            Dr = Cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            if(Dr.Read())
            {
                return Dr.GetValue(0).ToString();
            }
            else
            {
                return "";
            }
        }

        // 运行存储过程,返回 DataSet对象
        public static DataSet Proc_Get_DataSet(string procName, SqlParameter[] prams, string ConnStr, DataSet Ds)
        {
            SqlCommand Cmd = Proc_Create_Cmd(procName, prams, ConnStr);
            SqlDataAdapter Da = new SqlDataAdapter(Cmd);
            try
            {
                Da.Fill(Ds);
            } 
            catch(Exception Ex)
            {
                throw Ex;
            }
            return Ds;
        }
        */
        #endregion
    }
}
