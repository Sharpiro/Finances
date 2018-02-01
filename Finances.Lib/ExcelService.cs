using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Finances.Core
{
    public class ExcelService : ISpreadsheetService
    {
        public IEnumerable<Dictionary<string, string>> ReadSheet(string filePath)
        {
            const string sharedStringsPath = "xl/sharedStrings.xml";
            const string sheet1Path = "xl/worksheets/sheet1.xml";

            using (var zipArchive = ZipFile.OpenRead(filePath))
            {
                var sharedStringsList = GetSharedStrings(zipArchive);

                var sheet1Entry = zipArchive.GetEntry(sheet1Path);
                using (var sheet1Stream = sheet1Entry.Open())
                {
                    var sharedStringsDocument = XDocument.Load(sheet1Stream);
                    if (sharedStringsDocument.Root == null) throw new XmlException($"Unable to access root element of {sheet1Entry.Name}");
                    var xmlNs = sharedStringsDocument.Root.Name.Namespace;
                    var rowElements = sharedStringsDocument.Root.Descendants(xmlNs + "row").ToList();
                    var columnNames = new List<string>();

                    var rows = new List<Dictionary<string, object>>();
                    foreach (var rowElement in rowElements)
                    {
                        var rowCells = rowElement.Descendants(xmlNs + "c")
                            .Select(c =>
                            {
                                var isString = c.Attribute("t")?.Value == "s";
                                var parsedValue = int.TryParse(c.Value, out int cellValue);
                                return isString ? sharedStringsList[cellValue] : (object)cellValue;
                            });
                        if (columnNames.Count == 0)
                            columnNames = rowCells.Select(c => c as string).ToList();
                        else
                            yield return rowCells.Select((c, i) => new { c, i })
                                .ToDictionary(kvp => columnNames[kvp.i], kvp => kvp.c.ToString());
                        //rows.Add(rowCells.Select((c, i) => new { c, i }).ToDictionary(kvp => columnNames[kvp.i], kvp => kvp.c));
                    }
                }
            }

            List<string> GetSharedStrings(ZipArchive zipArchive)
            {
                var sharedStringsEntry = zipArchive.GetEntry(sharedStringsPath);
                using (var sharedStringsStream = sharedStringsEntry.Open())
                {
                    var sharedStringsDocument = XDocument.Load(sharedStringsStream);
                    if (sharedStringsDocument.Root == null) throw new XmlException($"Unable to access root element of {sharedStringsEntry.Name}");
                    var xmlNs = sharedStringsDocument.Root.Name.Namespace;
                    var sharedStringsList = sharedStringsDocument.Root.Descendants(xmlNs + "t")
                        .Select(e => e.Value).ToList();
                    return sharedStringsList;
                }
            }
        }

        public byte[] WriteSheet(IEnumerable<Dictionary<string, string>> data)
        {
            throw new NotImplementedException();
        }
    }
}