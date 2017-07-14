using System.Collections.Generic;

namespace Finances.Core
{
    public interface ISpreadsheetReader
    {
        IEnumerable<Dictionary<string, string>> ReadSheet(string filePath);
    }
}