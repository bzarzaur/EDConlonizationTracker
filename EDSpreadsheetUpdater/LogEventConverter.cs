using EDSpreadsheetUpdater.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8600
#pragma warning disable CS8603
#pragma warning disable CS8765

namespace EDSpreadsheetUpdater
{
    public class LogEventConverter : JsonConverter
    { 
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Journal);
        }


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            string eventType = obj["event"]?.ToString();

            Journal journal;
            switch (eventType)
            {
                case "CargoTransfer":
                    journal = obj.ToObject<CargoTransfer>(serializer);
                    break;
                default:
                    journal = new Journal();
                    break;
            }
            return journal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}