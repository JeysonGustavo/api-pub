using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.Pub.Core.Managers.Authentication;
using Api.Pub.Core.Models;
using Api.Pub.Core.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Pub.Application.Controllers
{
    [Route("authenticate")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        #region Objects
        private readonly IAuthenticateManager AuthenticateManager;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AuthenticateManager"></param>
        public AuthenticationController(IAuthenticateManager authenticateManager)
        {
            AuthenticateManager = authenticateManager;
        }
        #endregion

        #region Login
        /// <summary>
        /// Do Login
        /// </summary>
        /// <param name="login">Login object</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        [Produces(typeof(TokenModel))]
        [SwaggerResponse((int)HttpStatusCode.Created, typeof(TokenModel), "Inserted Success")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, null, "Bad formatted request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, null, "Authentication Error")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, null, "Conflict")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "API Error")]
        public async Task<IActionResult> Login([FromBody]LoginModel login)
        {
            var token = await AuthenticateManager.LoginByEmail(login);

            return Ok(token);
        }
        #endregion

        #region Login
        /// <summary>
        /// Create New User
        /// </summary>
        /// <param name="newUser">New User object</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("newUser")]
        [Produces(typeof(ResultModel))]
        [SwaggerResponse((int)HttpStatusCode.Created, typeof(ResultModel), "Inserted Success")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, null, "Bad formatted request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, null, "Authentication Error")]
        [SwaggerResponse((int)HttpStatusCode.Conflict, null, "Conflict")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, null, "API Error")]
        public async Task<IActionResult> CreateUser([FromBody]NewUserModel newUser)
        {
            var token = await AuthenticateManager.CreateUser(newUser);

            return Ok(token);
        }
        #endregion
    }
}