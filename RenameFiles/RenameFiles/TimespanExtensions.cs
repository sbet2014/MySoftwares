using System;
using System.Threading;

//Foundatio.Extensions

namespace RenameIamgeFiles
{
    public static class TimespanExtensions
    {
        public static TimeSpan Max(this TimeSpan source, TimeSpan other)
        {
            return source.Ticks < other.Ticks ? other : source;
        }
    }
}
