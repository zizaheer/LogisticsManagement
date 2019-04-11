using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace LogisticsManagement_BusinessLogic
{
    public class App_UserLogic : BaseLogic<App_UserPoco>
    {
        public App_UserLogic(IDataRepository<App_UserPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<App_UserPoco> GetList()
        {
            return base.GetList();
        }

        public override List<App_UserPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override App_UserPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public App_UserPoco GetSingleByUserName(string userName)
        {
            return _repository.GetSingle(d => d.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Add/Update/Remove Methods

        public override App_UserPoco Add(App_UserPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            poco.Password = GetBase64String(poco.Password);
            return base.Add(poco);
        }

        public override App_UserPoco Update(App_UserPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            poco.Password = GetBase64String(poco.Password);
            return base.Update(poco);
        }

        public override void Remove(App_UserPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(App_UserPoco[] pocos)
        {
            foreach (var poco in pocos)
            {
                poco.Password = GetBase64String(poco.Password);
            }
            base.Add(pocos);
        }

        public override void Update(App_UserPoco[] pocos)
        {
            foreach (var poco in pocos)
            {
                poco.Password = GetBase64String(poco.Password);
            }
            base.Update(pocos);
        }

        public override void Remove(App_UserPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion

        #region App Logic/Rules

        public bool IsCredentialsValid(string userName, string password, out App_UserPoco outUserData)
        {
            bool result = false;

            var userData = GetSingleByUserName(userName);
            outUserData = userData;

            if (userData != null)
            {
                if (userData.IsActive)
                {
                    if (ComparePassword(password, userData.Password))
                    {
                        result = true;
                        outUserData = userData;
                    }
                }
            }

            return result;
        }

        private bool ComparePassword(string suppliedValue, string storedValue)
        {
            if (GetBase64String(suppliedValue) == storedValue)
                return true;
            else
                return false;
        }

        private string GetBase64String(string value)
        {
            var byteArrayValue = GetPassowrdHash(value);
            return Convert.ToBase64String(byteArrayValue);
        }

        private static byte[] GetPassowrdHash(string value)
        {
            HashAlgorithm hashAlgorithm = SHA256.Create();
            var byteArrayValue = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(value));

            return byteArrayValue;
        }

        #endregion

    }
}
