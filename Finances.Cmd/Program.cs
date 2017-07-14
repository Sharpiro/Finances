using System;
using System.Linq;
using Finances.Core;
using Newtonsoft.Json;

namespace Finances.Cmd
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string csvFilePath = @"C:\Users\U403598\Desktop\temp\Chase0365_Activity_20170713.CSV";
            //const string excelFilePath = @"C:\Users\U403598\Desktop\temp\Chase0365_Activity_20170713.xlsx";
            const string excelFilePath = @"C:\Users\U403598\Desktop\temp\testExcelToXml.xlsx";
            


            ISpreadsheetReader sheetReader = new ExcelReader();
            //var temp = excelReader.ReadSheet(excelFilePath);
            //var csvReader = new CsvReader();
            var categoryStore = new CategoryDataStore();
            var rowData = sheetReader.ReadSheet(excelFilePath)
                .Select(r => new TransactionDto
                {
                    Type = r["Type"],
                    TransDate = DateTime.Parse(r["Trans Date"]),
                    PostDate = DateTime.Parse(r["Post Date"]),
                    Description = r["Description"],
                    Amount = decimal.Parse(r["Amount"]) * -1
                }).ToList();

            var groupedInfo = rowData.GroupBy(r => r.Description)
                .Select(g => new
                {
                    Description = g.Key,
                    RecordCount = g.Count(),
                    Sum = g.Sum(x => x.Amount),
                    DescriptionTransactions = g.OrderBy(a => a.TransDate).ToList()
                })
                .GroupBy(a => categoryStore.GetCategory(a.Description)).Select(g => new
                {
                    Category = g.Key,
                    RecordCount = g.Count(),
                    CategorySum = g.Sum(x => x.Sum),
                    CategoryTransactions = g.OrderBy(x => x.Description).ToList()
                })
                .OrderBy(a => a.Category).ToList();

            var json = JsonConvert.SerializeObject(groupedInfo);


        }
    }
}