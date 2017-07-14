using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Finances.Core
{
    public class CategoryDataStore : IEnumerable<KeyValuePair<string, IImmutableSet<string>>>
    {
        private readonly IImmutableDictionary<string, IImmutableSet<string>> _categoryDictionary =
            new Dictionary<string, IImmutableSet<string>>
            {
                ["Social"] = new HashSet<string>
                {
                    "lyft",
                    "uber",
                    "craft tasting",
                    "whiskey",
                    "tyber",
                    "street tavern",
                    "workman's friend",
                    "wokman's",
                    "sycamore",
                    "gin mill",
                    "pop the top",
                    "slate",
                    "central coffee",
                    "hot taco",
                    "pikes",
                    "fidelli",
                    "abc",
                    "roxbury",
                    "seoul",
                    "dandelion market",
                    "204 north"
                }.ToImmutableHashSet(),
                ["Fitness"] = new HashSet<string>
                {
                    "fitness"
                }.ToImmutableHashSet(),
                ["Payments"] = new HashSet<string>
                {
                    "automatic payment"
                }.ToImmutableHashSet(),
                ["Grocery"] = new HashSet<string>
                {
                    "publix",
                    "shipt",
                    "teeter"
                }.ToImmutableHashSet(),
                ["Lunch"] = new HashSet<string>
                {
                    "emeraldcommons"
                }.ToImmutableHashSet(),
                ["Entertainment"] = new HashSet<string>
                {
                    "amazon video",
                    "amazon digital svcs",
                    "netflix",
                    "hbo",
                    "xbox"
                }.ToImmutableHashSet(),
                ["Car"] = new HashSet<string>
                {
                    "speedway", "bp", "shell", "griffin bros", "geico", "exxon"
                }.ToImmutableHashSet(),
                ["Computing"] = new HashSet<string>
                {
                    "msft", "onedrive"
                }.ToImmutableHashSet(),
                ["Improvement"] = new HashSet<string>
                {
                    "sam ash", "audible", "zumiez", "kindle", "inline warehouse"
                }.ToImmutableHashSet(),
                ["Misc"] = new HashSet<string>
                {
                    "supercuts"
                }.ToImmutableHashSet(),
                ["Amazon Purchases"] = new HashSet<string>
                {
                    "amazon mktplace pmts", "amazon.com"
                }.ToImmutableHashSet()
            }.ToImmutableDictionary();

        public string GetCategory(string description)
        {
            foreach (var categoryPair in _categoryDictionary)
            {
                if (categoryPair.Value.Any(item => description.ToLowerInvariant().Contains(item)))
                    return categoryPair.Key;
            }
            return null;
        }

        public IEnumerator<KeyValuePair<string, IImmutableSet<string>>> GetEnumerator()
        {
            return _categoryDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}