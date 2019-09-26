using System.IdentityModel.Tokens.Jwt;

namespace Api.Pub.Core.Token
{
    public class TokenDecoder
    {
        #region Properties
        private JwtSecurityToken _token;
        private JwtSecurityTokenHandler _handler;
        #endregion

        #region Constructor
        public TokenDecoder(string accessToken)
        {
            _handler = new JwtSecurityTokenHandler();
            _token = Decode(accessToken);
        }
        #endregion

        #region Get Token
        public JwtSecurityToken GetToken()
        {
            return _token;
        }
        #endregion

        #region Decoder
        private JwtSecurityToken Decode(string accessToken)
        {
            var token = _handler.ReadJwtToken(accessToken);

            return token;
        } 
        #endregion
    }
}
