using System;
using System.Collections.Generic;
using TSG.Dal.Users;
using TSG.Models.APIModels;
using TSG.Models.DTO;
using TSG.Models.DTO.UsersDataBlock;
using TSG.Models.ServiceModels;
using TSG.Models.ServiceModels.UsersDataBlock;
using WinstantPay.Common.Interfaces;
using WinstantPay.Common.Object;

namespace TSG.ServiceLayer.Users
{
    public class UsersServiceMethods : IUsersServiceMethods
    {
        private readonly IUsersDalMethods _usersDalMethods;

        public UsersServiceMethods(IUsersDalMethods usersDalMethods)
        {
            _usersDalMethods = usersDalMethods;
        }

        public IResult<User> GetById(string id)
        {
            var res = _usersDalMethods.GetById(id);
            if (!res.Success || res.Obj == null)
                return new Result<User>(res.Message);
            return new Result<User>(AutoMapper.Mapper.Map<User>(res.Obj));

        }

        public IResult<List<User>> GetAllUsers()
        {
            var res = _usersDalMethods.GetAllUsers();
            if (!res.Success || res.Obj == null)
                return new Result<List<User>>(res.Message);
            return new Result<List<User>>(AutoMapper.Mapper.Map<List<User>>(res.Obj));
        }

        public IResult<User> InsertOrUpdateInfo(UserInfo ui) => _usersDalMethods.InsertOrUpdateInfo(ui);
        public IResult SaveUserAliases(UserAliasesSo userAliases) => _usersDalMethods.SaveUserAliases(AutoMapper.Mapper.Map<UserAliasesDto>(userAliases));

        public IResult<UserAliasesSo> ExistedUserByAliasName(string aliasName)
        {
            var query = _usersDalMethods.ExistedUserByAliasName(aliasName);
            return new Result<UserAliasesSo>(AutoMapper.Mapper.Map<UserAliasesSo>(query.Obj), query.Message);
        }

        public bool IfAliasExistInSystem(string aliasName, string userName)
        {
            var res = _usersDalMethods.ExistedUserByAliasName(aliasName);
            return res.Success && res.Obj != null;
        }

        public IResult<UserAliasesSo> GetUserAliasesByUserId(Guid userId)
        {
            var res = _usersDalMethods.GetUserAliasesByUserId(userId);
            return new Result<UserAliasesSo>(AutoMapper.Mapper.Map<UserAliasesSo>(res.Obj), res.Message);
        }

        public IResult<UserAliasesSo> GetUserAliasesByUserName(string userName)
        {
            var res = _usersDalMethods.GetUserAliasesByUserName(userName);
            return new Result<UserAliasesSo>(AutoMapper.Mapper.Map<UserAliasesSo>(res.Obj), res.Message);
        }

        public IResult Update(User user) => _usersDalMethods.Update(AutoMapper.Mapper.Map<UserDto>(user));
        public IResult UpdateAdmin(string firstName, string lastName, string email, string userName) => _usersDalMethods.UpdateAdmin(firstName, lastName, email, userName);
    }
}