using System;
using System.Collections.Generic;


namespace SmartVault.Shared.Utils
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> RandomDays()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}
