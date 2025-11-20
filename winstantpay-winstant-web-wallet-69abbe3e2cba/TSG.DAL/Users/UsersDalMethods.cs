using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using TSG.Models.APIModels;
using TSG.Models.DTO;
using TSG.Models.DTO.UsersDataBlock;
using TSG.Models.ServiceModels;
using WinstantPay.Common.DbExtensions;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;
using Role = TSG.Models.ServiceModels.Role;

namespace TSG.Dal.Users
{
    public class UsersDalMethods : IUsersDalMethods
    {
        private const string query = @"SELECT       u.username,
                                                    u.password,
                                                    u.KYCLink,
                                                    u.wlcMsgId,
                                                    u.lastLoginDate,
                                                    u.isLocal,
                                                    u.curLoginDate,
                                                    u.roleId,
                                                    u.userIdByTSG,
                                                    u.firstName,
                                                    u.lastName,
                                                    u.userMail,
                                                    u.needToSearchWelcomeMessage,
                                                    u.UserUiVersion,
                                                    u.IsViewedChangeLog,
                                                    r.roleId,
                                                    r.roleName,
                                                    r.roleDesc
                                        FROM dbo.[User] AS u INNER JOIN dbo.Role AS r ON r.roleId = u.roleId";

        public IResult<User> GetById(string id)
        {
            // Id == username
            using (var connection = ConnectionFactory.GetConnection())
            {
                var sql = query + " WHERE username = @id";
                var res = connection.Query<UserDto, RoleDto, User>(sql, (dto, roleDto) =>
                {
                    User u = new User();
                    u = AutoMapper.Mapper.Map<User>(dto);
                    if (u != null)
                        u.User_Role = AutoMapper.Mapper.Map<Role>(roleDto);
                    return u;
                }, new {id}, splitOn: "roleId").FirstOrDefault();

                return new Result<User>(res, res == null ? "Object not found" : String.Empty);
            }
        }

        public IResult<List<User>> GetAllUsers()
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var res = connection.Query<UserDto, RoleDto, User>(query, (dto, roleDto) =>
                {
                    User u = new User();
                    u = AutoMapper.Mapper.Map<User>(dto);
                    if (u != null)
                        u.User_Role = AutoMapper.Mapper.Map<Role>(roleDto);
                    return u;
                }, splitOn: "roleId").ToList();
                return new Result<List<User>>(res);
            }
        }

        public IResult<User> InsertOrUpdateInfo(UserInfo ui)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                var res = connection.Query<User, WelcomeMessageDto, RoleDto, User>(sql: "AddOrUpdateUser",
                    map: (user, wlcMsg, role) => new User
                    {
                        User_WelcomeMessage = AutoMapper.Mapper.Map<WelcomeMessage>(wlcMsg),
                        User_Role = AutoMapper.Mapper.Map<Role>(role),
                        User_IsNewUser = user.User_IsNewUser,
                        User_NeedToSearchWelcomeMessage = user.User_NeedToSearchWelcomeMessage,
                        User_CurLoginDate = user.User_CurLoginDate,
                        User_FirstName = user.User_FirstName,
                        User_LastName = user.User_LastName,
                        User_LastLoginDate = user.User_LastLoginDate,
                        User_IsLocal = user.User_IsLocal,
                        User_IsViewedChangeLog = user.User_IsViewedChangeLog,
                        User_KYCLink = user.User_KYCLink,
                        User_Password = user.User_Password,
                        User_RoleId = user.User_RoleId,
                        User_UserIdByTSG = user.User_UserIdByTSG,
                        User_UserMail = user.User_UserMail,
                        User_UserUiVersion = user.User_UserUiVersion,
                        User_Username = user.User_Username,
                        User_WlcMsgId = user.User_WlcMsgId,
                    },
                    param: new
                    {
                        username = ui.UserName,
                        userId = ui.UserId,
                        userEmail = ui.EmailAddress,
                        firstName = ui.FirstName,
                        lastName = ui.LastName,
                    }, commandType: CommandType.StoredProcedure, splitOn: "wlcMsgId, roleId").FirstOrDefault();

                return new Result<User>(res, res == null ? "Db Error" : String.Empty);
            }

            return null;
        }

        public IResult<UserAliasesDto> GetUserAliasesByUserId(Guid userId)
        {
            if (userId == Guid.Empty) return new Result<UserAliasesDto>(null, "Invalid user data");

            using (var connection = ConnectionFactory.GetConnection())
            {
                return new Result<UserAliasesDto>(connection.QueryFirstOrDefault<UserAliasesDto>("SELECT [UserId],[UserName],[UserAliases] FROM [dbo].[usr_UserAliases] WHERE UserId = @userId",
                        new { userId }, commandType: CommandType.Text));
            }
        }

        public IResult<UserAliasesDto> GetUserAliasesByUserName(string userName)
        {
            if (String.IsNullOrEmpty(userName)) return new Result<UserAliasesDto>("Invalid user data");

            using (var connection = ConnectionFactory.GetConnection())
            {
                return new Result<UserAliasesDto>(connection.QueryFirstOrDefault<UserAliasesDto>("SELECT [UserId],[UserName],[UserAliases] FROM [dbo].[usr_UserAliases] WHERE UserName = @userName",
                    new { userName }, commandType: CommandType.Text));
            }
        }
        
        public IResult SaveUserAliases(UserAliasesDto userAliases)
        {
            var cheeckUserIfExist = GetUserAliasesByUserId(userAliases.UserId);
            using (var connection = ConnectionFactory.GetConnection())
            {
                if (cheeckUserIfExist.Success && cheeckUserIfExist.Obj != null && cheeckUserIfExist.Obj.UserId != Guid.Empty)
                    return connection.UpdateResult(userAliases);
                return connection.InsertResult(userAliases);
            }
        }
        
        public IResult<UserAliasesDto> ExistedUserByAliasName(string aliasName)
        {
            if (String.IsNullOrEmpty(aliasName)) return new Result<UserAliasesDto>(null, "Alias name can't be empty");

            using (var connection = ConnectionFactory.GetConnection())
            {
                return new Result<UserAliasesDto>(
                    connection.QueryFirstOrDefault<UserAliasesDto>("SELECT [UserId],[UserName],[UserAliases] FROM [dbo].[usr_UserAliases] WHERE UserAliases LIKE N'%\"'+ @userAlias +'\"%'",
                        new { userAlias = aliasName.ToLower() }, commandType: CommandType.Text));
            }
        }

        public IResult Update(UserDto user)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.UpdateResult(user);
            }
        }

        public IResult UpdateAdmin(string firstName, string lastName, string email, string userName)
        {
            using (var connection = ConnectionFactory.GetConnection())
            {
                return connection.ExecuteResult("UPDATE dbo.[User] SET userMail = @email, firstName = @firstName, lastName=@lastName WHERE username = @userName", 
                    new {email, firstName, lastName, userName});
            }
        }
    }
}