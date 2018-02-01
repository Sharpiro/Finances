using System;

namespace Finances.Core
{
    public class SpreadSheetException : Exception
    {
        public SpreadSheetException(string message) : base(message) { }
    }
}