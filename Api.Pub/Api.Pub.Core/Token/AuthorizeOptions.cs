using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Api.Pub.Core.Token
{
    public class AuthorizeOptions
    {
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; }
        public DateTime NotBefore => DateTime.UtcNow;

        /// <summary>
        /// Token Creation
        /// </summary>
        public DateTime CreatedOn => DateTime.UtcNow;
        /// <summary>
        /// Duration of each session
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(40);
        /// <summary>
        /// Token Expiration
        /// </summary>
        public DateTime Expiration => CreatedOn.Add(ValidFor);
        /// <summary>
        /// JWT ID
        /// </summary>
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString());
        /// <summary>
        /// Key to generate token
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
