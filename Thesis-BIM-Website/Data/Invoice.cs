using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Data
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Invoice
    {
        [JsonProperty]
        [JsonConverter(typeof(UserConverter))]
        public virtual User User { get; set; }
        [JsonProperty]
        public int Id { get; set; }
        [JsonProperty]
        public string CompanyName { get; set; }
        [JsonProperty]
        public string BankAccountNumber { get; set; }
        [JsonProperty]
        public long Ocr { get; set; }
        [JsonProperty]
        public decimal AmountToPay { get; set; }
        [JsonProperty]
        public DateTime Paydate { get; set; }

    }

    public class UserConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(User);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
        object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            User user = value as User;
            serializer.Serialize(writer, new
            {
                user.Id,
                Username = user.UserName,
                user.Email
            });
        }
    }
}
