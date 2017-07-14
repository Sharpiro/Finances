using System;

namespace Finances.Core
{
    public class TransactionDto
    {
        public string Type { get; set; }
        public DateTime TransDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }
}