﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Finances.Core
{
    public class CsvReader : ISpreadsheetReader
    {
        public IEnumerable<Dictionary<string, string>> ReadSheet(string filePath)
        {
            var csvData = File.ReadAllText(filePath);
            var rows = csvData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var firstRow = rows.FirstOrDefault();
            if (firstRow == null) throw new SpreadSheetException("Unable to determine first row of spreadsheet");
            var columnNames = firstRow.Split(',');
            var columnCount = columnNames.Length;

            var parsedRows = rows.Skip(1).Select((r, i) =>
                {
                    var data = r.Split(',');
                    if (data.Length != columnCount) throw new SpreadSheetException($"Invalid number of columns @ row {i}.  Expected: {columnCount}. Actual: {data.Length}");
                    return data.Select((d, j) => new KeyValuePair<string, string>(columnNames[j], d))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            );
            return parsedRows;
        }
    }
}