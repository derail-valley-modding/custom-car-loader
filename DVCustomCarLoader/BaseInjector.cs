using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader
{
    public class BaseInjector
    {
        public const int CUSTOM_TYPE_OFFSET = 0x4000_0000;
        public const int CUSTOM_TYPE_MASK = CUSTOM_TYPE_OFFSET - 1;
        public const int CUSTOM_TYPE_MAX_VALUE = CUSTOM_TYPE_OFFSET + CUSTOM_TYPE_MASK;

        public static T GenerateUniqueType<T>(string identifier, Func<T, bool> checkRegistered)
            where T : Enum
        {
            int hash = identifier.GetHashCode();
            hash = (hash & CUSTOM_TYPE_MASK) + CUSTOM_TYPE_OFFSET;

            for (int searchWatchdog = CUSTOM_TYPE_MASK; searchWatchdog >= 0; searchWatchdog--)
            {
                T candidate = (T)Enum.ToObject(typeof(T), hash);
                if (!checkRegistered(candidate))
                {
                    return candidate;
                }

                // move up by 1 and try again
                hash += 1;
                if (hash > CUSTOM_TYPE_MAX_VALUE) hash = CUSTOM_TYPE_OFFSET;
            }

            Main.Error("No available custom types, something is VERY wrong");
            return default;
        }
    }
}
