using CCL.Types;
using DV.ThingTypes;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer.Types
{
    public class CCL_CarType : TrainCarType_v2
    {
        public IEnumerable<CCL_CarVariant> Variants => liveries.OfType<CCL_CarVariant>();

        public DVTrainCarKind KindSelection;
        public TranslationData NameTranslations = new();
        public LoadableCargo? CargoTypes;
    }
}
