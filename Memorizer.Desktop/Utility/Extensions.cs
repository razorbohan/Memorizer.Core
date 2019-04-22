using System.Reflection;
using WPFLocalizeExtension.Extensions;

namespace Memorizer.Utility
{
    public static class Extensions
    {
        public static string GetLocalizedValue(string key)
        {
            return LocExtension.GetLocalizedValue<string>(Assembly.GetCallingAssembly().GetName().Name + ":Resources:" + key);
        }
    }
}
