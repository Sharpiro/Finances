using System.Collections.Generic;

namespace Finances.Core
{
    public interface ISpreadsheetService
    {
        IEnumerable<Dictionary<string, string>> ReadSheet(string filePath);
        byte[] WriteSheet(IEnumerable<Dictionary<string, string>> data);
    }
}