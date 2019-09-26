using Api.Pub.Core.Models;
using Api.Pub.Core.Models.Authentication;
using System.Threading.Tasks;

namespace Api.Pub.Core.Providers
{
    public interface IDataBaseProvider
    {
        #region Authentication
        /// <summary>
        /// Do Login
        /// </summary>
        /// <param name="login">Login object</param>
        /// <returns></returns>
        Task<UserModel> LoginByEmail(LoginModel login);

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="newUser">New User object</param>
        /// <returns></returns>
        Task<ResultModel> CreateUser(NewUserModel newUser);

        /// <summary>
        /// Password recovery
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="password">New password/param>
        /// <param name="cpf">Cpf of the user</param>
        /// <returns></returns>
        Task<ResultModel> ForgotPassword(string email, string password, string cpf);
        #endregion
    }
}
