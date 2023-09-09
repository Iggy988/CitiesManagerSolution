using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.Services;

public class JwtService : IJwtService
{

    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AuthenticationResponse CreateJwtToken(ApplicationUser user)
    {
        DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

        /*
         A claim is a statement about a subject by an issuer. 
        Claims represent attributes of the subject that are useful in the context of authentication and authorization operations. 
        Subjects and issuers are both entities that are part of an identity scenario. 
        Some typical examples of a subject are: a user, an application or service, a device, or a computer. 
        Some typical examples of an issuer are: the operating system, an application, a service,
        a role provider, an identity provider, or a federation provider. An issuer delivers claims by issuing security tokens,
        typically through a Security Token Service (STS). (In WIF, you can build an STS by deriving from the SecurityTokenService class.)
        On occasion, the collection of claims received from an issuer can be extended by subject attributes stored directly at the resource.
        A claim can be evaluated to determine access rights to data and other secured resources during the process
        of authorization and can also be used to make or express authentication decisions about a subject. 
         */

       
        Claim[] claims = new Claim[] {
            // values that we put in payload
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject (user id)

            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //JWT unique ID

            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), //Issued at (date and time of token generation)

            new Claim(ClaimTypes.NameIdentifier, user.Email), //Unique name identifier of the user (Email)

            new Claim(ClaimTypes.Name, user.PersonName) //Name of the user
        };

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //Sha = securedHashingAlgoritm
        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        //tokenGenerator -> create token based on data we input above
        JwtSecurityToken tokenGenerator = new JwtSecurityToken(
             _configuration["Jwt:Issuer"],
             _configuration["Jwt:Audience"],
             claims,
             expires: expiration,
             signingCredentials: signingCredentials
        );

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        string token = tokenHandler.WriteToken(tokenGenerator);


        return new AuthenticationResponse() { Token = token, Email = user.Email, PersonName = user.PersonName, Expiration = expiration };

    }
}
