using System.Collections.Generic;

namespace Finances.Core
{
    public interface ICategoryDataStore: IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>
    {
        (string Category, string Description) GetCategory(string description);
    }
}