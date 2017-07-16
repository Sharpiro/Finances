using System;
using System.IO;

namespace Finances.Core
{
    public class SpreadsheetFactory
    {
        public ISpreadsheetService CreateSpreadsheetService(SpreadSheetType spreadSheetType)
        {
            return spreadSheetType == SpreadSheetType.Csv ?
                (ISpreadsheetService)new CsvService() : new ExcelService();
        }

        public ISpreadsheetService CreateSpreadsheetServiceFromFilePath(string filePath)
        {
            var spreadsheetType = filePath.EndsWith("csv", StringComparison.OrdinalIgnoreCase) ?
                SpreadSheetType.Csv : filePath.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase) ?
                SpreadSheetType.Excel : throw new IOException($"Invalid file extension");

            return CreateSpreadsheetService(spreadsheetType);
        }
    }

    public enum SpreadSheetType { Csv, Excel }
}