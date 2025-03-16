#pragma warning disable CS8618 
namespace EDSpreadsheetUpdater.Models
{
    public class Journal
    {
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
    }

    public class CargoTransfer : Journal
    {
        public List<Transfer> Transfers { get; set; }
    }

    public class Transfer
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public string Direction { get; set; }
    }

}
