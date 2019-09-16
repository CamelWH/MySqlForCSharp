using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MySqlForCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            MySqlHelper.ConnectDataBase("root", "root", "localhost", "skypath", "3306");
            Console.WriteLine("连接成功");
            //创建一个表
            //Dictionary<string, string> dict = new Dictionary<string, string>();
            //dict.Add("user", "text");
            //dict.Add("id", "int");
            //MySqlHelper.CreateTable("testTable", dict);
            //删除一个表
            //MySqlHelper.DeleteTable("testTable");
            //添加数据
            //MySqlHelper.AddData("testTable", new string[] { "user", "id" }, new string[] { "'神'", "1" });
            //MySqlHelper.AddData("testTable", new string[] { "user", "id" }, new string[] { "'人'", "2" });
            //MySqlHelper.AddData("testTable", new string[] { "user", "id" }, new string[] { "'仙'", "3" });
            //读取数据
            //MySqlHelper.ReadTable("testTable");
            //删除数据
            //MySqlHelper.DeleteData("testTable", "user", "=", "'人'");
            //更新数据
            MySqlHelper.UpdateData("testTable", "user", "'仙'", "id", "=", "1");
        }
    }
}
