using CCL.Types;
using DV.Customization.Paint;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer
{
    internal static class PaintLoader
    {

        public static void LoadSubstitutions(IEnumerable<PaintSubstitutions> substitutions)
        {
            foreach (var substitution in substitutions)
            {
                if (!PaintTheme.TryLoad(substitution.Paint, out var theme))
                {
                    CCLPlugin.Error($"Could not find paint {substitution.Paint} to prepare substitutions");
                    continue;
                }

                var list = theme.substitutions.ToList();

                foreach (var item in substitution.Substitutions)
                {
                    list.Add(new PaintTheme.Substitution
                    {
                        original = item.Original,
                        substitute = item.Substitute
                    });
                }

                theme.substitutions = list.ToArray();
            }
        }
    }
}
