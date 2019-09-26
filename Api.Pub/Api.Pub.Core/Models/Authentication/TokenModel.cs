using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Pub.Core.Models.Authentication
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long ExpiresIn { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
