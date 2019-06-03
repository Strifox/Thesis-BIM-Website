using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thesis_BIM_Website.Data;
using Thesis_BIM_Website.Interfaces;
using Thesis_BIM_Website.Models;
using Thesis_BIM_Website.Services;

namespace Thesis_BIM_Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        const string subscriptionKey = "d2e3fa02640a41d482f7b399e6673579";
        const string uriBase =
        "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/ocr";

        public InvoicesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RequestToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            return Ok();

        }

        // GET: api/Invoices
        [Route("GetInvoices")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices(string userId)
        {
            var invoices = _context.Invoices.Where(x => x.UserId == userId).ToListAsync();

            return Ok(await invoices);
        }


        [Route("AnalyzeImage")]
        [HttpPost]
        public IActionResult AnalyzeImage([FromBody]Invoice invoice)
        {
            if (!string.IsNullOrEmpty(invoice.Base64String) && !string.IsNullOrWhiteSpace(invoice.Base64String))
            {
                string json = MakeComputerVisionRequest(invoice.Base64String).Result;
                OcrResults result = JsonConvert.DeserializeObject<OcrResults>(json);
                string sanitisedString = OcrResultsToString(result);

                return Ok(InvoiceFromString(sanitisedString));
            }
            return BadRequest(new { message = "Image can't be null" });
        }

        [AllowAnonymous]
        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] Invoice input)
        {
            var user = await _context.Users.Where(x => x.Id == input.UserId).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("That user does not exist");
            }

            var invoice = new Invoice
            {
                User = (User)user,
                CompanyName = input.CompanyName,
                Ocr = input.Ocr,
                BankAccountNumber = input.BankAccountNumber,
                AmountToPay = input.AmountToPay,
                Paydate = input.Paydate
            };

            _context.Add(invoice);
            await _context.SaveChangesAsync();

            var invoiceJson = JsonConvert.SerializeObject(invoice, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            return CreatedAtAction("CreateInvoice", invoiceJson);
        }

        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return BadRequest();
            }

            _context.Entry(invoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Invoice>> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return invoice;
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }

        /// <summary>
        /// Makes an request to Azure Computer Vision
        /// </summary>
        /// <param name="base64string">Base64 encoded image</param>
        /// <returns>Json string containing text and text boundries found in the image</returns>
        [HttpPost]
        private static async Task<string> MakeComputerVisionRequest(string base64string)
        {
            try
            {
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                string requestParameters = "language=unk";

                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                byte[] byteData = Convert.FromBase64String(base64string);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    response = await client.PostAsync(uri, content);
                }

                string contentString = await response.Content.ReadAsStringAsync();

                return contentString;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Matches OCR-Number, AmountToPay and (Due date)* *if it's in the right format
        /// </summary>
        /// <param name="input">a string that only contains the text fields from the OcrResult object</param>
        /// <returns>Invoice with Ocr-Number, AmountToPay and (Due date)* *if it's in the right format</returns>
        private Invoice InvoiceFromString(string input)
        {
            const string bankAccountNumberPattern = @"\b[\d]{3}-[\d]{4}\b";
            const string ocrNumberAndAmountPattern = @"\b[\d]{10,16}\s[#\s]{0,2}[\d]{1,4}\s[\d]{2}\b";
            const string dueDatePattern = @"\b[\d]{4}(-[\d]{2}){2}\b";
            
            Invoice invoice = new Invoice();

            Match match = Regex.Match(input, bankAccountNumberPattern);
            if (match.Success)
            {
                invoice.BankAccountNumber = match.Captures[0].Value;
            }

            match = Regex.Match(input, ocrNumberAndAmountPattern);

            if (match.Success)
            {
                string OcrAndAmount = match.Captures[0].Value;

                string[] splitStrings;

                //If the computer vision does not find the # sign it will make a whitespace instead
                if (OcrAndAmount.Contains('#'))
                {
                    splitStrings = OcrAndAmount.Split('#');
                }
                else
                {
                    Regex regex = new Regex(@"\s");
                    //We only want to split at the first occurance of a whitespace
                    splitStrings = regex.Split(OcrAndAmount, 2);
                }

                string Ocr = splitStrings[0];
                string amountToPay = splitStrings[1].Substring(0, splitStrings[1].IndexOf(' ')).Trim() + "," + splitStrings[1].Substring(splitStrings[1].IndexOf(' ')).Trim();

                invoice.Ocr = Convert.ToInt64(Ocr);
                invoice.AmountToPay = Convert.ToDecimal(amountToPay, new CultureInfo("sv-SE"));
            }

            var matches = Regex.Matches(input, dueDatePattern);

            if (matches.Count >= 2)
            {
                //We don't know which date is the due date so we name them date1 and date2
                DateTime date1 = DateTime.Parse(matches[0].Value);
                DateTime date2 = DateTime.Parse(matches[1].Value);

                if (date1 >= date2)
                {
                    invoice.Paydate = date1;
                }
                else
                {
                    invoice.Paydate = date2;
                }
            }

            return invoice;
        }

        private string OcrResultsToString(OcrResults result)
        {
            return string.Join("\n",
                result.Regions.ToList().Select(region =>
                    string.Join(" ", region.Lines.ToList().Select(line =>
                         string.Join(" ", line.Words.ToList().Select(word =>
                             word.Text).ToArray())).ToArray())).ToArray());
        }
    }
}
