using System;
using System.Linq;
using Finances.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using static System.Console;

namespace Finances.Cmd
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var executingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var workingDirectory = Directory.GetCurrentDirectory();
                var filePath = args.ElementAtOrDefault(0);
                var outputFilePath = args.ElementAtOrDefault(1) ?? $"{workingDirectory}/out.csv";
                var outputJsonFilePath = args.ElementAtOrDefault(1) ?? $"{workingDirectory}/out.json";

                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!File.Exists(filePath)) throw new FileNotFoundException($"No file found @ '{filePath}'");
                }
                else
                {
                    var inputFiles = Directory.GetFiles(workingDirectory)
                        .Where(f => f.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase) || f.EndsWith("csv", StringComparison.OrdinalIgnoreCase))
                        .Where(f => !f.EndsWith("out.csv", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    if (inputFiles.Count == 0) throw new IOException($"No files found in working directory: '{workingDirectory}'");
                    if (inputFiles.Count > 1) throw new IOException($"More than one spreadsheet file was found in working directory: '{workingDirectory}'");
                    filePath = inputFiles.Single();
                }

                WriteLine($"ifp: '{filePath}'");
                WriteLine($"ofp: '{outputFilePath};");
                WriteLine($"ojfp: '{outputJsonFilePath};");

                var spreadSheetFactory = new SpreadsheetFactory();
                var sheetService = spreadSheetFactory.CreateSpreadsheetServiceFromFilePath(filePath);

                WriteLine($"ExecutingDir: '{executingDirectory}'");
                WriteLine($"WorkingDir: '{workingDirectory}'");

                ICategoryDataStore categoryStore = new CategoryDataStore();
                var rowData = sheetService.ReadSheet(filePath)
                    .Select(r => new TransactionDto
                    {
                        Type = r["Type"],
                        TransDate = DateTime.Parse(r["Trans Date"]),
                        PostDate = DateTime.Parse(r["Post Date"]),
                        Description = r["Description"],
                        Amount = decimal.Parse(r["Amount"]) * -1
                    }).ToList();

                var groupedInfo = rowData.GroupBy(r => r.Description)
                    .Select(g =>
                    {
                        var categoryData = categoryStore.GetCategory(g.Key);
                        return new
                        {
                            Category = categoryData.Category,
                            Description = categoryData.Description,
                            RecordCount = g.Count(),
                            Sum = g.Sum(x => x.Amount),
                            DescriptionTransactions = g.Select(x => new TransactionDto
                            {
                                Type = x.Type,
                                TransDate = x.TransDate,
                                PostDate = x.PostDate,
                                Description = x.Description,
                                GenericDescription = categoryData.Description,
                                Amount = x.Amount,
                                Category = categoryData.Category
                            }).OrderBy(a => a.TransDate).ToList()
                        };
                    })
                    .GroupBy(a => a.Category).Select(g => new
                    {
                        Category = g.Key,
                        RecordCount = g.Count(),
                        CategorySum = g.Sum(x => x.Sum),
                        CategoryTransactions = g.OrderBy(x => x.Description).ToList()
                    })
                    .OrderBy(a => a.Category);

                var flattenedData = groupedInfo.SelectMany(g => g.CategoryTransactions)
                    .SelectMany(cts => cts.DescriptionTransactions
                        .Select(tdto => new TransactionDto
                        {
                            Type = tdto.Type,
                            TransDate = tdto.TransDate,
                            PostDate = tdto.PostDate,
                            Description = tdto.Description,
                            GenericDescription = tdto.GenericDescription,
                            Amount = tdto.Amount,
                            Category = cts.Category
                        })
                )
                .Where(d => !d.Category?.Equals("payments", StringComparison.OrdinalIgnoreCase) ?? true);

                var writeAbleData = flattenedData.Select(fd => new Dictionary<string, string>
                {
                    [nameof(fd.Type)] = fd.Type,
                    [nameof(fd.TransDate)] = fd.TransDate.ToString(),
                    [nameof(fd.PostDate)] = fd.PostDate.ToString(),
                    [nameof(fd.Description)] = fd.Description,
                    [nameof(fd.GenericDescription)] = fd.GenericDescription,
                    [nameof(fd.Amount)] = fd.Amount.ToString(),
                    [nameof(fd.Category)] = fd.Category,
                });

                var json = JsonConvert.SerializeObject(groupedInfo, Formatting.Indented);
                var data = sheetService.WriteSheet(writeAbleData);
                File.WriteAllBytes(outputFilePath, data);
                File.WriteAllText(outputJsonFilePath, json);
            }
            catch (Exception ex)
            {
                WriteLine($"Error: '{ex.Message}'");
            }
        }
    }
}