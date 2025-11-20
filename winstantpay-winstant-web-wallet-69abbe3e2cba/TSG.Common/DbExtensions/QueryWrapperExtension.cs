using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace WinstantPay.Common.DbExtensions
{
    public static class QueryWrapperExtension
    {
        public static IResult<List<T>> QueryResult<T>(this SqlConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                return new Result<List<T>>(connection.Query<T>(sql, param, commandType: commandType).ToList());
            }
            catch (Exception ex)
            {
                return new Result<List<T>>(ex.Message);
            }
        }

        public static IResult<T> QuerySingleResult<T>(this SqlConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                return new Result<T>(connection.QuerySingle<T>(sql, param, commandType: commandType));
            }
            catch (Exception ex)
            {
                return new Result<T>(ex.Message);
            }
        }

        public static IResult<T> QueryFirstOrDefaultResult<T>(this SqlConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                var res = connection.Query<T>(sql, param, commandType: commandType).FirstOrDefault();
                if(res == null)
                    throw new Exception("Object not found");
                return new Result<T>(res);
            }
            catch (Exception ex)
            {
                return new Result<T>(ex.Message);
            }
        }

        public static IResult<T> QueryReturnResult<T>(this SqlConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                return new Result<T>(connection.Query<T>(sql, param, commandType: commandType).Single());
            }
            catch (Exception ex)
            {
                return new Result<T>(ex.Message);
            }
        }

        public static IResult ExecuteResult(this SqlConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                connection.Execute(sql, param, commandType: commandType);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }

        public static IResult<T> InsertResult<T>(this SqlConnection connection, T model) where T : class
        {
            try
            {
                connection.Insert(model);
                return new Result<T>(model);
            }
            catch (Exception ex)
            {
                return new Result<T>(ex.Message);
            }
        }

        public static IResult InsertResult<T>(this SqlConnection connection, List<T> models) where T : class
        {
            try
            {
                connection.Insert(models);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }

        public static IResult<bool> QueryBooleanResult(this SqlConnection connection, string sql, object param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                return new Result<bool>(connection.QuerySingle<int>(sql, param, commandType: commandType) > 0);
            }
            catch (Exception ex)
            {
                return new Result<bool>(ex.Message);
            }
        }

        public static IResult DeleteResult<T>(this SqlConnection connection, T model) where T : class
        {
            try
            {
                return connection.Delete(model) ? new Result() : new Result("Ошибка удаления");
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }

        public static IResult DeleteResult<T>(this SqlConnection connection, List<T> models) where T : class
        {
            try
            {
                return connection.Delete(models) ? new Result() : new Result("Ошибка удаления");
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }

        public static IResult UpdateResult<T>(this SqlConnection connection, T model) where T : class
        {
            try
            {
                connection.Update(model);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }

        public static IResult UpdateResult<T>(this SqlConnection connection, List<T> models) where T : class
        {
            try
            {
                connection.Update(models);
                return new Result();
            }
            catch (Exception ex)
            {
                return new Result(ex.Message);
            }
        }
    }
}
