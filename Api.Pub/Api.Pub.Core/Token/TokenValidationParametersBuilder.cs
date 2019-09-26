using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Api.Pub.Core.Token
{
    public class TokenValidationParametersBuilder
    {
        #region Properties
        private bool _ValidateIssuer;
        private string _ValidIssuer;
        private bool _ValidateAudience;
        private string _ValidAudience;
        private bool _ValidateIssuerSigningKey;
        private SecurityKey _IssuerSigningKey;
        private bool _RequireExpirationTime;
        private bool _ValidateLifetime;
        private TimeSpan _ClockSkew;
        #endregion

        #region Contructor
        /// <summary>
        /// Parameters that will be used in TokenValidation
        /// </summary>
        /// <param name="tokenOptions"></param>
        /// <param name="signingKey"></param>
        public TokenValidationParametersBuilder(IConfigurationSection tokenOptions, SymmetricSecurityKey signingKey)
        {
            _ValidateIssuer = false;
            _ValidIssuer = tokenOptions[nameof(AuthorizeOptions.Issuer)];
            _ValidateAudience = false;
            _ValidAudience = tokenOptions[nameof(AuthorizeOptions.Audience)];
            _ValidateIssuerSigningKey = true;
            _IssuerSigningKey = signingKey;
            _RequireExpirationTime = true;
            _ValidateLifetime = true;
            _ClockSkew = TimeSpan.Zero;
        }
        #endregion

        #region Token Build
        /// <summary>
        /// Create a new instance of TokenValidationParameters
        /// </summary>
        /// <returns></returns>
        public TokenValidationParameters Build()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = _ValidateIssuer,
                ValidIssuer = _ValidIssuer,

                ValidateAudience = _ValidateAudience,
                ValidAudience = _ValidAudience,

                ValidateIssuerSigningKey = _ValidateIssuerSigningKey,
                IssuerSigningKey = _IssuerSigningKey,

                RequireExpirationTime = _RequireExpirationTime,
                ValidateLifetime = _ValidateLifetime,

                ClockSkew = _ClockSkew
            };
        } 
        #endregion
    }
}
