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
using Thesis_BIM_Website.Models;

namespace Thesis_BIM_Website.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
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

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            return await _context.Invoices.ToListAsync();
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        [Route("AnalyzeImage")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AnalyzeImage([FromBody]Invoice invoice)
        {
            if (!string.IsNullOrEmpty(invoice.Base64String) && !string.IsNullOrWhiteSpace(invoice.Base64String))
            {
                var json = JsonConvert.DeserializeObject<OcrResults>(MakeOCRRequest(invoice.Base64String).Result);

                var x = OcrResultsToString(json);

                //return Ok(json);


                return Ok(GetTextFromString(x));
            }

            return BadRequest(new { message = "Image can't be null" });
        }

        [Route("Create")]
        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice(string userId, string companyName, long ocr, string bankaccountnumber, decimal amountToPay, DateTime paydate)
        {
            var user = await _context.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("That user does not exist");
            }

            var invoice = new Invoice { User = (User)user, CompanyName = companyName, Ocr = ocr, BankAccountNumber = bankaccountnumber, AmountToPay = amountToPay, Paydate = paydate };
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

        static async Task<string> MakeOCRRequest(string base64string)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. 
                // The language parameter doesn't specify a language, so the 
                // method detects it automatically.
                // The detectOrientation parameter is set to true, so the method detects and
                // and corrects text orientation before detecting text.
                string requestParameters = "language=unk";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = Convert.FromBase64String(base64string);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                return contentString;

                // Display the JSON response.
                //Console.WriteLine("\nResponse:\n\n{0}\n",
                //	JToken.Parse(contentString).ToString());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }


        public Invoice GetTextFromString(string input)
        {
            Invoice invoice = new Invoice();
            var match = Regex.Match(input, @"\b[\d]{3}-[\d]{4}\b");
            if (match.Success)
            {
                invoice.BankAccountNumber = match.Captures[0].Value;
            }

            match = Regex.Match(input, @"\b[\d]{10,16}\s[#\s]{0,2}[\d]{1,4}\s[\d]{2}\b");

            if (match.Success)
            {
                string temp = match.Captures[0].Value.ToString();

                Regex regex = new Regex(@"\s");
                string[] sub = regex.Split(temp, 2);

                if (temp.Contains('#'))
                {
                    sub = temp.Split('#');
                }

                string amountToPay = sub[1].Substring(0, sub[1].IndexOf(' ')).Trim() + "," + sub[1].Substring(sub[1].IndexOf(' ')).Trim();

                invoice.Ocr = Convert.ToInt64(sub[0]);
                invoice.AmountToPay = Convert.ToDecimal(amountToPay, new CultureInfo("sv-SE"));
            }

            var matches = Regex.Matches(input, @"\b[\d]{4}(-[\d]{2}){2}\b");

            if (matches.Count >= 2)
            {
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

        public string OcrResultsToString(OcrResults result)
        {
            return string.Join("\n",
                result.Regions.ToList().Select(region =>
                    string.Join(" ", region.Lines.ToList().Select(line =>
                         string.Join(" ", line.Words.ToList().Select(word =>
                             word.Text).ToArray())).ToArray())).ToArray());
        }
    }
}
