using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Thesis_BIM_Website.Data;
using Thesis_BIM_Website.Interfaces;
using Thesis_BIM_Website.Models;

namespace Thesis_BIM_Website.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserManagementService _userManagementService;
        private readonly TokenManagement _tokenManagement;
        private readonly ApplicationDbContext _context;

        public TokenAuthenticationService(IUserManagementService service, IOptions<TokenManagement> tokenManagement, ApplicationDbContext context)
        {
            _userManagementService = service;
            _tokenManagement = tokenManagement.Value;
            _context = context;
        }
        public bool IsAuthenticated(TokenRequest request, out string token)
        {

            token = string.Empty;
            if (!_userManagementService.IsValidUser(request.Username, request.Password))
                return false;

            User user = _context.Users.Where(x => x.UserName == request.Username).FirstOrDefault();
            var claim = new[]
            {
                new Claim("userId", user.Id),
                new Claim("username", request.Username)
            };
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                _tokenManagement.Issuer,
                _tokenManagement.Audience,
                claim,
                expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
                signingCredentials: credentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;

        }
    }
}
