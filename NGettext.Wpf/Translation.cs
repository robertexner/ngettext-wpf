using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using NGettext.Wpf.Serialization;

namespace NGettext.Wpf
{
    public static class Translation
    {
        private static TranslationSerializer _translationSerializer;

        [StringFormatMethod("msgId")]
        public static string GetText(string msgId, params object[] @params)
        {
            return @params.Any() ? Localizer.Gettext(msgId, @params) : Localizer.Gettext(msgId);
        }

        public static ILocalizer Localizer { get; set; }

        public static string Noop(string msgId) => msgId;

        //[StringFormatMethod("singularMsgId")]
        //[StringFormatMethod("pluralMsgId")] //< not yet supported, #1833369.
        //[Obsolete("Use GetPluralString() instead.  This method will be removed in 2.x")]
        //public static string PluralGettext(int n, string singularMsgId, string pluralMsgId, params object[] @params)
        //{
        //    return GetPluralString(singularMsgId, pluralMsgId, n, @params);
        //}

        //[StringFormatMethod("singularMsgId")]
        //[StringFormatMethod("pluralMsgId")] //< not yet supported, #1833369.
        //public static string GetPluralString(string singularMsgId, string pluralMsgId, int n, params object[] args)
        //{
        //    if (Translation.Localizer is { })
        //    {
        //        return args.Any()
        //            ? Translation.Localizer.Catalog.GetPluralString(singularMsgId, pluralMsgId, n, args)
        //            : Translation.Localizer.Catalog.GetPluralString(singularMsgId, pluralMsgId, n);
        //    }

        //    CompositionRoot.WriteMissingInitializationErrorMessage();
        //    return string.Format(CultureInfo.InvariantCulture, n == 1 ? singularMsgId : pluralMsgId, args);
        //}

        //[StringFormatMethod("text")]
        //[StringFormatMethod("pluralText")] //< not yet supported, #1833369.
        //public static string GetParticularPluralString(string context, string text, string pluralText, int n, params object[] args)
        //{
        //    if (Translation.Localizer is { })
        //    {
        //        return args.Any()
        //            ? Translation.Localizer.Catalog.GetParticularPluralString(context, text, pluralText, n, args)
        //            : Translation.Localizer.Catalog.GetParticularPluralString(context, text, pluralText, n);
        //    }

        //    CompositionRoot.WriteMissingInitializationErrorMessage();
        //    return string.Format(CultureInfo.InvariantCulture, n == 1 ? text : pluralText, args);
        //}

        //[StringFormatMethod("text")]
        //public static string GetParticularString(string context, string text, params object[] args)
        //{
        //    if (Translation.Localizer is { })
        //    {
        //        return args.Any()
        //            ? Translation.Localizer.Catalog.GetParticularString(context, text, args)
        //            : Translation.Localizer.Catalog.GetParticularString(context, text);
        //    }

        //    CompositionRoot.WriteMissingInitializationErrorMessage();
        //    return (args.Any() ? string.Format(CultureInfo.InvariantCulture, text, args) : text);
        //}

#if ALPHA

        //[StringFormatMethod("msgId")]
        //[Obsolete("This method is experimental, and may go away")]
        //public static string SerializedGettext(IEnumerable<CultureInfo> cultureInfos, string msgId, params object[] args)
        //{
        //    if (Localizer is null)
        //    {
        //        var message = args.Any() ? Localizer.Gettext(msgId, args) : Localizer.Gettext(msgId);

        //        return "{" + string.Join(", ", cultureInfos.Select(c => $"\"{c.Name}\": \"{HttpUtility.JavaScriptStringEncode(message)}\"")) + "}";
        //    }

        //    if (_translationSerializer == null)
        //    {
        //        _translationSerializer = new TranslationSerializer(Localizer.GetCatalog);
        //    }

        //    return _translationSerializer.SerializedGettext(cultureInfos, msgId, args);
        //}
#endif
    }
}