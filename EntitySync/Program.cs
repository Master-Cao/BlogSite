using System.Data;
using System.Net;
using System.Reflection;

using Site.Domain.Entities;
using SqlSugar;

var db = new SqlSugarClient(new ConnectionConfig()
{
    ConnectionString = "Server=gz-cynosdbmysql-grp-5ltngzx3.sql.tencentcdb.com;Port=23104;Database=site;Uid=root;Pwd=199910Cao;",
    DbType = SqlSugar.DbType.MySql,
    IsAutoCloseConnection = true,
    InitKeyType = InitKeyType.Attribute,
    ConfigureExternalServices = new ConfigureExternalServices
    {
        //注意:  这儿AOP设置不能少
        EntityService = (c, p) =>
        {
            /***低版本C#写法***/
            // int?  decimal?这种 isnullable=true 不支持string(下面.NET 7支持)
            if (p.IsPrimarykey == false && c.PropertyType.IsGenericType &&
            c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                p.IsNullable = true;
            }

            /***高版C#写法***/
            //支持string?和string  
            if (p.IsPrimarykey == false && new NullabilityInfoContext()
             .Create(c).WriteState is NullabilityState.Nullable)
            {
                p.IsNullable = true;
            }
        }
    }
});

#region Aop
//db.Aop.OnLogExecuting = (sql, pars) =>
//{
//    Console.WriteLine(sql + "" + Db.Utilities.SerializeObject
//        (pars.ToDictionary(it => it.ParameterName, it => it.Value)));
//    Console.WriteLine();
//}; 
#endregion

#region 对话框
Console.WriteLine("是否确定同步数据库表结构？(同名数据表将会被备份，生产环境慎用，回车确认)");
var str = Console.ReadKey();
if (str.Key == ConsoleKey.Enter)
{
    Console.WriteLine("同步中，请稍后...");
}
else
{
    Console.WriteLine("\r\n输入错误，已退出...");
    return;
}
#endregion

//同步数据表结构
db.DbMaintenance.CreateDatabase();
db.CodeFirst.SetStringDefaultLength(255).BackupTable().InitTables(new Type[]
{
   //typeof(Plans.PlansModel),
   //typeof(SiteImages),
   typeof(Blogs)
   //typeof(BlogTags)
   //typeof(DefaultImage)
});
Console.WriteLine("数据库结构同步完成!");
