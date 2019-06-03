using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Memorizer.Web.Models.Authentication
{
    public class JwtAuthentication
    {
        public string SecurityKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }

        public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecurityKey));
        public SigningCredentials SigningCredentials => new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }
}
