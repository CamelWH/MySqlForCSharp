using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace MySqlForCSharp
{
    public class MySqlHelper
    {
        public static MySqlConnection connect;
        public static MySqlCommand command;
        public static MySqlDataReader dataReader;
        /// <summary>
        /// 连接数据库
        /// MySqlHelper.ConnectDataBase("root", "root", "localhost", "skypath", "3306");
        /// Server=xxx;UserId=yyy;Password=zzz;Database=dbdb
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="server">数据库地址</param>
        /// <param name="database">数据库名称</param>
        /// <param name="port">端口号</param>
        public static void ConnectDataBase(string user, string password, string server, string database, string port)
        {
            try
            {
                if (connect != null)
                    connect.Close();
                string str = "user=" + user + ";password=" + password + ";server=" + server + ";port=" + port + ";database=" + database + ";";
                Console.WriteLine(str);
                connect = new MySqlConnection(str);
                connect.Open();
                command = connect.CreateCommand();
                Console.WriteLine("连接数据库成功!");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public static void CloseConnect()
        {
            //销毁Command
            if (command != null)
            {
                command.Cancel();
            }
            command = null;

            //销毁Reader
            //if (dataReader != null)
            //{
            //    dataReader.Close();
            //}
            //dataReader = null;

            //销毁Connection
            if (connect != null)
            {
                connect.Close();
            }
            connect = null;
        }
        /// <summary>
        /// 向数据库添加一个表,列类型为string
        /// Dictionary<string, string> dict = new Dictionary<string, string>();
        /// dict.Add("user", "text");
        /// MySqlHelper.CreateTable("testTable", dict);
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="colNames">列名称</param>
        public static void CreateTable(string tableName, Dictionary<string, string> colNameType)
        {
            string content = string.Empty;
            foreach (var item in colNameType)
            {
                content += item.Key + " " + item.Value + ",";
            }
            content = content.Remove(content.Length - 1);
            string opcode = "create table " + tableName + "(" + content + ")" + ";";
            Console.WriteLine(opcode);
            ExecuteQuery(opcode);
        }
        /// <summary>
        /// 删除一个表
        /// </summary>
        /// <param name="tableName"></param>
        public static void DeleteTable(string tableName)
        {
            string opcode = "drop table " + tableName + ";";
            ExecuteQuery(opcode);
        }
        /// <summary>
        /// 向一个表中添加数据
        /// MySqlHelper.AddData("testTable", new string[] { "user", "id" }, new string[] { "'abc'", "123" });
        /// text型:需要添加''
        /// </summary>
        public static void AddData(string tableName, string[] colName, string[] values)
        {
            string content = "(" + string.Join(",", colName) + ")";
            string value = "(" + string.Join(",", values) + ")";
            string opcode = "insert into " + tableName + content + "values" + value;
            Console.WriteLine(opcode);
            ExecuteQuery(opcode);
        }
        /// <summary>
        /// 更新表中的某个数据
        /// MySqlHelper.UpdateData("testTable", "user", "'仙'", "id", "=", "1");
        /// </summary>
        public static void UpdateData(string tableName, string colName, string value, string key,string op,string keyValue)
        {
            string queryString = string.Format("update {0} set {1}={2} where {3}{4}{5};", tableName, colName, value, key, op, keyValue);
            Console.WriteLine(queryString);
            ExecuteQuery(queryString);
        }
        /// <summary>
        /// 删除表中某个数据
        /// MySqlHelper.DeleteData("testTable", "user", "=", "'人'");
        /// </summary>
        public static void DeleteData(string tableName, string colName, string operation, string colValue)
        {
            string queryString = "delete from " + tableName + " where " + colName + " " + operation+ colValue + ";";
            Console.WriteLine(queryString);
            ExecuteQuery(queryString);
        }
        /// <summary>
        /// 读取整张表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static Table ReadTable(string tableName)
        {
            string queryString = "select*from " + tableName;
            Table table = new Table();
            table.Change(tableName);
            int lineNum = 1;
            using (command = new MySqlCommand(queryString, connect))
            {
                try
                {
                    dataReader=command.ExecuteReader();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        table.colNames.Add(dataReader.GetName(i));
                    }
                    while (dataReader.Read())
                    {
                        for (int i = 0; i < dataReader.FieldCount; i++)
                        {
                            Element element = new Element();
                            element.Change(lineNum, dataReader.GetName(i), dataReader[dataReader.GetName(i)]);
                            Console.WriteLine(element);
                            table.AddElement(element);
                        }
                        lineNum++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine(table);
            return table;
        }
        /// <summary>
        /// 执行指令
        /// </summary>
        private static void ExecuteQuery(string queryString)
        {
            using (command = new MySqlCommand(queryString, connect))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        /// <summary>
        /// 用户密码验证
        /// </summary>
        public static bool VerifyUser(string userName, string password)
        {
            string sqlstr = "select * from czhenya001 where username = @para1 and password = @para2";
            command = new MySqlCommand(sqlstr, connect);
            command.Parameters.AddWithValue("para1", userName);
            command.Parameters.AddWithValue("para2", password);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 自制的数据表
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string tableName;
        /// <summary>
        /// 列名
        /// </summary>
        public List<string> colNames;
        /// <summary>
        /// 该表的所有元素
        /// </summary>
        public Dictionary<int, List<Element>> content;
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="lineNum"></param>
        /// <param name="e"></param>
        public void AddElement(Element e)
        {
            if (!content.ContainsKey(e.lineNum))
            {
                content.Add(e.lineNum, new List<Element>());
            }
            content[e.lineNum].Add(e);
        }
        public Table()
        {
        }
        public void Change(string tableName)
        {
            this.tableName = tableName;
            this.colNames = new List<string>();
            this.content = new Dictionary<int, List<Element>>();
        }
        public override string ToString()
        {
            string str="表名:"+tableName+"\n"+"列名:"+string.Join(",",colNames);
            foreach (var item in content)
            {
                str += "\n" + string.Join("----", item.Value);
            }
            return str;
        }
    }
    /// <summary>
    /// 表中一个元素
    /// </summary>
    public class Element
    {
        /// <summary>
        /// 该元素的的内容
        /// </summary>
        public object content;
        /// <summary>
        /// 该元素的行号
        /// </summary>
        public int lineNum;
        /// <summary>
        /// 该元素的列名
        /// </summary>
        public string colName;
        public Element()
        {
        }
        public void Change(int lineNum, string colName, object content)
        {
            this.lineNum = lineNum;
            this.colName = colName;
            this.content = content;
        }
        public override string ToString()
        {
            return string.Format("行号:{0};列名:{1};内容:{2};类型:{3}", lineNum, colName, content,content.GetType());
        }
    }
}
