using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace AffixEditor
{
    class AffixTypeManager
    {
        public static void  AddAffixType(AffixType type)
        {
            // This gets the _Types filed of AffixType by reference
            HashSet<AffixType> types = typeof(AffixType).GetField("_Types", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as HashSet<AffixType>;
            if (!types.Contains(type))
            {
                types.Add(type);

                // TODO: Make not hardcoded
                using (StreamWriter writer = File.AppendText("Data\\Affix\\AffixTypes"))
                {
                    writer.WriteLine(type);
                }
            }
        }
    }
}
