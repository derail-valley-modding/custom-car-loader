using CCL.Types;
using DV.ThingTypes;
using DVLangHelper.Data;

namespace CCL.Importer.Types
{
    public class CCL_CarVariant : TrainCarLivery
    {
        public TrainCarType BaseCarType;
        public TranslationData NameTranslations = new TranslationData();

        public bool UseCustomFrontBogie;
        public bool UseCustomRearBogie;

        public bool UseCustomBuffers;
        public bool UseCustomHosePositions;

        public bool HideFrontCoupler;
        public bool HideBackCoupler;
    }
}
