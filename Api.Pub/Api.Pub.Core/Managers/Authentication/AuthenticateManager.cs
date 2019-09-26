using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Api.Pub.Core.Models;
using Api.Pub.Core.Models.Authentication;
using Api.Pub.Core.Providers;
using Api.Pub.Core.Token;
using Microsoft.Extensions.Options;

namespace Api.Pub.Core.Managers.Authentication
{
    public class AuthenticateManager : IAuthenticateManager
    {
        #region Objects
        private AuthorizeOptions Options;
        private IDataBaseProvider DataBaseProvider;
        private UserModel User;
        private SHA256 Encryptor;
        private JwtSecurityTokenHandler Handler;
        #endregion

        #region Constructor
        public AuthenticateManager(IDataBaseProvider dataBaseProvider, IOptions<AuthorizeOptions> options)
        {
            DataBaseProvider = dataBaseProvider;
            Options = options.Value;
            User = new UserModel();
            Encryptor = SHA256.Create();
            Handler = new JwtSecurityTokenHandler();
        }
        #endregion

        #region Create User
        public async Task<ResultModel> CreateUser(NewUserModel newUser)
        {
            try
            {
                newUser.Password = GenerateHashMd5(newUser.Password);

                var result = await DataBaseProvider.CreateUser(newUser);

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Forgot Password
        public Task<ResultModel> ForgotPassword(string email, string password, string cpf)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Login
        public async Task<TokenModel> LoginByEmail(LoginModel login)
        {
            if (await ValidateUser(login))
            {
                try
                {
                    var identity = await GetIdentity(User);
                    var claims = await CreateClaims(identity);
                    var jwt = CreateJwtSecurityToken(claims);
                    var token = Handler.WriteToken(jwt);

                    return new TokenModel
                    {
                        TokenType = "Bearer",
                        AccessToken = token,
                        ExpiresIn = (int)Options.ValidFor.TotalSeconds,
                        CreatedDate = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException(ex.Message);
                }
            }
            else
                throw new UnauthorizedAccessException("Usuário ou senha inválidos.");
        }
        #endregion

        #region Validate User
        /// <summary>
        /// Check if the user is valid
        /// </summary>
        /// <param name="login">Login object</param>
        /// <returns></returns>
        private async Task<bool> ValidateUser(LoginModel login)
        {
            try
            {
                login.Password = GenerateHashMd5(login.Password);

                var user = await DataBaseProvider.LoginByEmail(login);

                if (user == null)
                    throw new UnauthorizedAccessException("Usuário ou Senha inválidos.");

                User.Id = user.Id;
                User.Email = user.Email;
                User.FirstName = user.FirstName;
                User.LastName = user.LastName;

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Não foi possível efetuar login");
            }
        }
        #endregion

        #region Encripty password
        private static string GenerateHashMd5(string password)
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
        #endregion

        #region Fill the Claim        
        /// <summary>
        /// Check the user and fill the claim
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private Task<ClaimsIdentity> GetIdentity(UserModel user)
        {
            var identity = new ClaimsIdentity(
                new GenericIdentity(user.Email, "Token"),
                new[] {
                    new Claim("WebApi", "User"),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                }
            );

            return Task.FromResult(identity);
        }
        #endregion

        #region Create the Claim        
        /// <summary>        
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        private async Task<Claim[]> CreateClaims(ClaimsIdentity identity)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, User.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, await Options.JtiGenerator()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(Options.CreatedOn).ToString(), ClaimValueTypes.Integer64));

            foreach (var item in identity.Claims)
            {
                claims.Add(item);
            }

            return claims.ToArray();
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        #endregion

        #region Create the Jwt Security and decode
        /// <summary>
        /// Create the JWT Token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private JwtSecurityToken CreateJwtSecurityToken(Claim[] claims)
        {
            var securityToken = new JwtSecurityToken(
                  issuer: Options.Issuer,
                  audience: Options.Audience,
                  claims: claims,
                  notBefore: Options.NotBefore,
                  expires: Options.Expiration,
                  signingCredentials: Options.SigningCredentials
                );

            return securityToken;
        }

        private JwtSecurityToken Decode(string accessToken)
        {
            var token = Handler.ReadJwtToken(accessToken);

            return token;
        }
        #endregion
    }
}
