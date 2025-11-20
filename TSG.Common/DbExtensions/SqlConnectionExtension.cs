using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using WinstantPay.Common.Object;
using WinstantPay.Common.Interfaces;

namespace WinstantPay.Common.DbExtensions
{
    public static class SqlConnectionExtension
    {
        public static IResult RunExecute<T>(this SqlConnection connection, string sql, T model, CommandType commandType = CommandType.Text)
        {
            try
            {
                connection.Execute(sql, model, commandType: commandType);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }
        public static IResult RunExecute(this SqlConnection connection, string sql, object model, CommandType commandType = CommandType.Text)
        {
            try
            {
                connection.Execute(sql, model, commandType: commandType);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }
        public static IResult RunExecute(this SqlConnection connection, string sql)
        {
            try
            {
                connection.Execute(sql);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }


    }
}
