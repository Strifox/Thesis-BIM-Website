using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Thesis_BIM_Website.Data;
using Thesis_BIM_Website.Interfaces;
using Thesis_BIM_Website.Models;

namespace Thesis_BIM_Website.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceManagementService _invoiceManagementService;
        private readonly TokenManagement _tokenManagement;
        private readonly ApplicationDbContext _context;

        public InvoiceService(IInvoiceManagementService service, IOptions<TokenManagement> tokenManagement, ApplicationDbContext context)
        {
            _invoiceManagementService = service;
            _tokenManagement = tokenManagement.Value;
            _context = context;
        }

        public bool IsAuthenticated(InvoiceRequest request, out string token)
        {
            var invoices = _context.Invoices.ToList();
            token = string.Empty;
            if (!_invoiceManagementService.IsValidUserId(request.UserId))
                return false;

            Claim[] claim = null;
            foreach (var invoice in invoices)
            {
                if (invoice.UserId == request.UserId)
                {
                    claim = new Claim[]
                       {
                new Claim("company", invoice.CompanyName ),
                new Claim("amount", invoice.AmountToPay.ToString()),
                new Claim("bankaccountnumber", invoice.BankAccountNumber),
                new Claim("ocr", invoice.Ocr.ToString()),
                new Claim("paydate", invoice.Paydate.ToString()),
            };
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
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
