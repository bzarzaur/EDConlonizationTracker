using EDSpreadsheetUpdater.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS0219 
#pragma warning disable CS8604 

class Program
{
    static string[] Scopes = { SheetsService.Scope.Spreadsheets };
    static string ApplicationName = "Elite Dangerous Colonization Material Tracking Updater";
    static string SpreadsheetId = "";
    static string SheetName = "";
    static string CredentialsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\GoogleSheetsApi";
    static long lastPosition = 0;

    static void Main()
    {
        Console.WriteLine("Welcome to the ED Colonization Spreadsheet Updater");
        Console.WriteLine("If you've read the README file then you should have the SpreadsheetId");
        Console.WriteLine("SpreadsheetId: ");
        SpreadsheetId = Console.ReadLine();
        Console.WriteLine("Workbook Name: ");
        SheetName = Console.ReadLine();
        var folderPath = "C:\\Users\\brand\\Saved Games\\Frontier Developments\\Elite Dangerous";

        try
        {
            var latestFile = GetLatestFile(folderPath);
            var journals = new List<Journal>();
            var service = AuthenticateGoogleSheets();

            using (var stream = new FileStream(latestFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                lastPosition = stream.Length;
            }

            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {
                watcher.Path = Path.GetDirectoryName(latestFile.FullName);
                watcher.Filter = Path.GetFileName(latestFile.FullName);
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
                watcher.Changed += async (s, e) => await ReadNewEntries(latestFile, service); // Handle updates
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }

            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading or deserializing JSON: " + ex.Message);
        }
    }

    private static FileInfo GetLatestFile(string folderPath)
    {
        var directoryInfo = new DirectoryInfo(folderPath);
        if (!directoryInfo.Exists)
        {
            throw new DirectoryNotFoundException($"The directory {folderPath} was not found.");
        }

        var files = directoryInfo.GetFiles();
        return files.Where(x => x.Extension == ".log").OrderByDescending(x => x.LastWriteTime).First();
    }

    static async Task ReadNewEntries(FileInfo latestFile, SheetsService service)
    {
        try
        {
            using (var stream = new FileStream(latestFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                stream.Seek(lastPosition, SeekOrigin.Begin); // Start at the last position

                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        ProcessJsonLine(line, service);
                    }
                }

                lastPosition = stream.Position; // Update last read position
            }
        }
        catch (IOException)
        {
            Console.WriteLine("File in use, retrying...");
        }
    }

    static void ProcessJsonLine(string jsonLine, SheetsService service)
    {
        try
        {
            var logEvent = JsonConvert.DeserializeObject<Journal>(jsonLine);
            if (logEvent != null)
            {
                if (logEvent.Event == "CargoTransfer")
                {
                    var cargoTransfer = JsonConvert.DeserializeObject<CargoTransfer>(jsonLine);
                    foreach (var cargo in cargoTransfer.Transfers)
                    {
                        UpdateColumnInRow(service, cargo.Type, 2, cargo.Count);
                    }
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON: {ex.Message}");
        }
    }

    private static SheetsService AuthenticateGoogleSheets()
    {
        var directoryInfo = new DirectoryInfo(CredentialsPath);
        if (!directoryInfo.Exists)
        {
            throw new DirectoryNotFoundException($"The directory {CredentialsPath} was not found.");
        }

        var files = directoryInfo.GetFiles();
        var path = files.Where(x => x.Extension == ".json").First();

        GoogleCredential credential;
        using (var stream = new FileStream(path.FullName, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        return new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    private static void UpdateColumnInRow(SheetsService service, string lookupValue, int columnIndex, int newValue)
    {
        string range = $"{SheetName}!A2:C20"; // Adjust to match your sheet's range
        var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);
        var response = request.Execute();
        var values = response.Values;

        if (values == null || values.Count == 0)
        {
            Console.WriteLine("No data found.");
            return;
        }

        int rowIndex = -1;
        int currentNumber = 0;
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i].Count > 0 && string.Equals(Regex.Replace(values[i][0].ToString(), @"\s+",""), lookupValue, StringComparison.OrdinalIgnoreCase)) // Searching in column A
            {
                rowIndex = i;
                currentNumber = int.Parse(values[i][2].ToString(), NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                break;
            }
        }

        if (rowIndex == -1)
        {
            Console.WriteLine("Row not found.");
            return;
        }

        // Construct the update range (e.g., C5 for rowIndex=4 and columnIndex=2)
        string updateRange = $"{SheetName}!{(char)('A' + columnIndex)}{rowIndex + 2}";

        var valueRange = new ValueRange
        {
            Values = new List<IList<object>> { new List<object> { newValue + currentNumber } }
        };

        var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, updateRange);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        updateRequest.Execute();

        Console.WriteLine($"Updated row {rowIndex + 2}, column {columnIndex + 1} with '{newValue}'.");
    }
}