using System;
using System.Globalization;

namespace NGettext.Wpf
{
    public class CultureEventArgs : EventArgs
    {
        public CultureEventArgs(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
        }

        public CultureInfo CultureInfo { get; }
    }
}