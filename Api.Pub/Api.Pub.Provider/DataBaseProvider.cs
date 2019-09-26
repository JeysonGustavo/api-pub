using Api.Pub.Core.Models;
using Api.Pub.Core.Models.Authentication;
using Api.Pub.Core.Providers;
using Dapper;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api.Pub.Provider
{
    public class DataBaseProvider : IDataBaseProvider
    {
        #region Get Connection String
        private static string GetConnectionString()
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace("Api.Pub.Application\\bin\\Debug\\netcoreapp2.2", "");
            return $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"{rootPath}Api.Pub.Provider\\AppData\\AppDataBase.mdf\";Integrated Security=True";
        }
        #endregion

        #region Authentication
        public async Task<ResultModel> CreateUser(NewUserModel newUser)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    var result = await conn.ExecuteScalarAsync<int>(@"INSERT INTO tbl_login(FirstName, LastName, Cpf, Email, Password)
                                                                      VALUES(@FirstName, @LastName, @Cpf, @Email, @Password); 
                                                                      SELECT SCOPE_IDENTITY();",
                                                                        new
                                                                        {
                                                                            FirstName = newUser.FirstName,
                                                                            LastName = newUser.LastName,
                                                                            Cpf = newUser.CPF,
                                                                            Email = newUser.Email,
                                                                            Password = newUser.Password,
                                                                        });

                    //success
                    if (result > 0)
                    {
                        return new ResultModel
                        {
                            Message = string.Empty,
                            ResultData = string.Empty,
                            Status = true
                        };
                    }
                    else
                    {
                        return new ResultModel
                        {
                            Message = "E-mail não encontrado.",
                            ResultData = string.Empty,
                            Status = false
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Message = $"Erro: {ex.Message}",
                    ResultData = string.Empty,
                    Status = false
                };
            }
        }

        public async Task<ResultModel> ForgotPassword(string email, string password, string cpf)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    var result = await conn.ExecuteScalarAsync<int>(@"UPDATE tbl_login 
                                                                      SET Password = @Password
                                                                      WHERE Email = @Email
                                                                        AND Cpf = @Cpf",
                                                                        new
                                                                        {
                                                                            Email = email,
                                                                            Password = password,
                                                                            Cpf = cpf
                                                                        });

                    //success
                    if (result > 0)
                    {
                        return new ResultModel
                        {
                            Message = string.Empty,
                            ResultData = string.Empty,
                            Status = true
                        };
                    }
                    else
                    {
                        return new ResultModel
                        {
                            Message = "E-mail não encontrado.",
                            ResultData = string.Empty,
                            Status = false
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    Message = $"Erro: {ex.Message}",
                    ResultData = string.Empty,
                    Status = false
                };
            }
        }

        public async Task<UserModel> LoginByEmail(LoginModel login)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    var result = (await conn.QueryAsync<UserModel>(@"SELECT Id, FirstName, LastName, Cpf AS CPF, Email
                                                                    FROM tbl_login 
                                                                    WHERE Email = @Email
                                                                      AND Password = @Password; ",
                                                                     new
                                                                     {
                                                                         Email = login.UserEmail,
                                                                         Password = login.Password
                                                                     })
                                   ).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        } 
        #endregion
    }
}
