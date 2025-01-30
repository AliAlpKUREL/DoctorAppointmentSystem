using DoctorAppointmentSystem.Model;
using DoctorAppointmentSystem.Service;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using MongoDB.Bson;
using System;
using System.Text;

namespace DoctorAppointmentSystem.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;
        private readonly MedicineService _medicineService;
        public Worker(ILogger<Worker> logger, HttpClient httpClient, MedicineService medicineService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _medicineService = medicineService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                var currentDateTime = DateTime.Now;

                //if (currentDateTime.DayOfWeek == DayOfWeek.Sunday && currentDateTime.Hour == 22 && currentDateTime.Minute == 0)
                {
                    await UpdateMedicineDataAsync();
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        public async Task<string> GetLatestFileUrlAsync()
        {
            var url = "https://www.titck.gov.tr/dinamikmodul/43";
            var html = await _httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var links = htmlDoc.DocumentNode.SelectNodes("//a[contains(@href, '.xlsx')]");

            if (links != null && links.Any())
            {
                var latestLink = links.Last();
                var fileUrl = latestLink.GetAttributeValue("href", string.Empty);
                return fileUrl;
            }

            return null;
        }

        private async Task UpdateMedicineDataAsync()
        {
            try
            {
                var fileUrl = await GetLatestFileUrlAsync();
                if (fileUrl is null)
                {
                    return;
                }
                var fileData = await _httpClient.GetByteArrayAsync(fileUrl);
                var filePath = Path.Combine(Path.GetTempPath(), "medicines.xlsx");
                await File.WriteAllBytesAsync(filePath, fileData);

                var medicines = ParseExcelFile(filePath);
               medicines.Remove(medicines.First(x => x.Name == "Ürün adý"));

                Random random = new();
                foreach (var medicine in medicines)
                {
                    medicine.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                    medicine.Price = random.Next(50, 500); 
                }

                foreach (var medicine in medicines)
                {
                    await _medicineService.CreateMedicineAsync(medicine);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private List<Medicine> ParseExcelFile(string filePath)
        {
            var medicines = new List<Medicine>();

            using (var spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                var workbookPart = spreadsheetDocument.WorkbookPart;
                var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                if (sheet == null) return medicines;

                var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);
                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();

                if (sheetData != null)
                {
                    foreach (var row in sheetData.Elements<Row>())
                    {
                        var cell = row.Elements<Cell>().ElementAtOrDefault(2);
                        if (cell != null)
                        {
                            var atcCode = GetCellValue(spreadsheetDocument, cell);
                            if (!string.IsNullOrEmpty(atcCode))
                            {
                                medicines.Add(new Medicine
                                {
                                    Name = atcCode 
                                });
                            }
                        }
                    }
                }
            }

            return medicines;
        }

        private string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            var value = cell.CellValue?.Text;
            if (value == null) return string.Empty;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var sharedStringTable = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                value = sharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(value)).Text.Text;
            }

            return value;
        }
    }
}
