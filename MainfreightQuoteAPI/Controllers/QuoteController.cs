using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainfreightQuoteAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuoteController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var values = new Dictionary<string, string>
            {
                { "deviceUuid", "00000000-0000-0000-0000-000000000000" },
                { "brand", "MFT" },
                { "platform", "Android" },
                { "quoteOfTheDayLastReadDate", null },
                { "loginLastReadDate", null },
                { "appSuggestionLastReadDate", null }
            };

            client.DefaultRequestHeaders.AcceptLanguage.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("en-GB"));

            var content = new StringContent(JsonSerializer.Serialize(values), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://mobile.mainfreight.com/mft/2.1/Home/GetInbox", content);

            var responseString = await response.Content.ReadAsStringAsync();

            var json = JsonSerializer.Deserialize<JsonElement>(responseString);

            var quote = json.GetProperty("cards").EnumerateArray().FirstOrDefault(c => c.GetProperty("type").GetString() == "QuoteOfTheDay").GetProperty("title").GetString();

            return Ok(quote);
        }
    }
}
