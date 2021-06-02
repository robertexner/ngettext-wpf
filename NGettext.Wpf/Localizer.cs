using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NGettext.Wpf
{
    public interface ILocalizer
    {
        IList<ICatalog> Catalogs { get; }

        ICultureTracker CultureTracker { get; }

        ICatalog GetCatalog(CultureInfo cultureInfo, string domainName = null);
    }

    public class Localizer : IDisposable, ILocalizer
    {
        private readonly IList<string> mDomainNames = new List<string>();

        public Localizer(ICultureTracker cultureTracker, string domainName)
        {
            mDomainNames.Add(domainName);
            CultureTracker = cultureTracker;
            if (cultureTracker == null) throw new ArgumentNullException(nameof(cultureTracker));
            cultureTracker.CultureChanging += ResetCatalog;
            ResetCatalog(cultureTracker.CurrentCulture);
        }

        public Localizer(ICultureTracker cultureTracker, IEnumerable<string> domainNames)
        {
            ((List<string>)mDomainNames).AddRange(domainNames);
            CultureTracker = cultureTracker;
            if (cultureTracker == null) throw new ArgumentNullException(nameof(cultureTracker));
            cultureTracker.CultureChanging += ResetCatalog;
            ResetCatalog(cultureTracker.CurrentCulture);
        }

        private void ResetCatalog(object sender, CultureEventArgs e)
        {
            ResetCatalog(e.CultureInfo);
        }

        private void ResetCatalog(CultureInfo cultureInfo)
        {
            if (Catalogs == null)
            {
                Catalogs = new List<ICatalog>();
            }
            else
            {
                Catalogs.Clear();
            }

            foreach (var domainName in mDomainNames)
            {
                Catalogs.Add(GetCatalog(cultureInfo, domainName));
            }
        }

        public ICatalog GetCatalog(CultureInfo cultureInfo, string domainName)
        {
            var localeDir = "Locale";
            Console.WriteLine(
                $"NGettext.Wpf: Attempting to load \"{Path.GetFullPath(Path.Combine(localeDir, cultureInfo.Name, "LC_MESSAGES", domainName + ".mo"))}\"");
            return new Catalog(domainName, localeDir, cultureInfo);
        }

        public IList<ICatalog> Catalogs { get; private set; }
        public ICultureTracker CultureTracker { get; }

        public void Dispose()
        {
            CultureTracker.CultureChanging -= ResetCatalog;
        }
    }

    public static class LocalizerExtensions
    {
        internal struct MsgIdWithContext
        {
            internal string Context { get; set; }
            internal string MsgId { get; set; }
        }

        internal static MsgIdWithContext ConvertToMsgIdWithContext(string msgId)
        {
            var result = new MsgIdWithContext { MsgId = msgId };

            if (!msgId.Contains("|"))
            {
                return result;
            }

            var pipePosition = msgId.IndexOf('|');
            result.Context = msgId.Substring(0, pipePosition);
            result.MsgId = msgId.Substring(pipePosition + 1);

            return result;
        }

        internal static string Gettext(this ILocalizer @this, string msgId, params object[] values)
        {
            if (msgId is null) return null;

            var msgIdWithContext = ConvertToMsgIdWithContext(msgId);

            if (@this is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return string.Format(msgIdWithContext.MsgId, values);
            }

            var text = msgIdWithContext.MsgId;
            var result = string.Empty;

            foreach (var catalog in @this.Catalogs)
            {
                if (msgIdWithContext.Context != null)
                {
                    result = catalog.GetParticularString(msgIdWithContext.Context, msgIdWithContext.MsgId, values);

                    if (result.Equals(text))
                    {
                        continue;
                    }

                    break;
                }

                result = catalog.GetString(msgIdWithContext.MsgId, values);

                if (result.Equals(text))
                {
                    continue;
                }

                break;
            }

            text = result;

            return text;
        }

        internal static string Gettext(this ILocalizer @this, string msgId)
        {
            if (msgId is null) return null;

            var msgIdWithContext = ConvertToMsgIdWithContext(msgId);

            if (@this is null)
            {
                CompositionRoot.WriteMissingInitializationErrorMessage();
                return msgIdWithContext.MsgId;
            }

            var text = msgIdWithContext.MsgId;
            var result = string.Empty;

            foreach (var catalog in @this.Catalogs)
            {
                if (msgIdWithContext.Context != null)
                {
                    result = catalog.GetParticularString(msgIdWithContext.Context, msgIdWithContext.MsgId);

                    if (result.Equals(text))
                    {
                        continue;
                    }

                    break;
                }

                result = catalog.GetString(msgIdWithContext.MsgId);

                if (result.Equals(text))
                {
                    continue;
                }

                break;
            }

            text = result;

            return text;
        }
    }
}