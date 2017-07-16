﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Finances.Core
{
    public class CategoryDataStore : ICategoryDataStore
    {
        private readonly IImmutableDictionary<string, IReadOnlyCollection<string>> _categoryDictionary =
            new Dictionary<string, IReadOnlyCollection<string>>
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
                    "payment"
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
                ["Amazon Purchases"] = new HashSet<string>
                {
                    "amazon mktplace pmts", "amazon.com"
                }.ToImmutableHashSet()
            }.ToImmutableDictionary();

        public (string Category, string Description) GetCategory(string description)
        {
            foreach (var categoryPair in _categoryDictionary)
            {
                foreach (var item in categoryPair.Value)
                {
                    if (description.ToLowerInvariant().Contains(item))
                        return (categoryPair.Key, item);
                }
            }
            return (null, description);
        }

        public IEnumerator<KeyValuePair<string, IReadOnlyCollection<string>>> GetEnumerator()
        {
            return _categoryDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}