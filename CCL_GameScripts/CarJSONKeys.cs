using System;
using System.Collections.Generic;
using System.Linq;

namespace CCL_GameScripts
{
    public class CarJSONKeys
    {
        public const string JSON_FILENAME = "car.json";
        public const string BUNDLE_NAME = "assetBundleName";
        public const string PREFAB_NAME = "carPrefabName";
        public const string IDENTIFIER = "identifier";
        public const string CAR_TYPE = "carType";

        public const string REPLACE_FRONT_BOGIE = "frontBogieReplacement";
        public const string FRONT_BOGIE_PARAMS = "frontBogieParams";

        public const string REPLACE_REAR_BOGIE = "rearBogieReplacement";
        public const string REAR_BOGIE_PARAMS = "rearBogieParams";

        // Cargo Definitions
        public const string CARGO_JSON_FILE = "cargo.json";
        public const string CARGO_SHORT_NAME = "shortName";
        public const string CARGO_LONG_NAME = "longName";
        public const string CARGO_MASS = "massPerUnit";
        public const string CARGO_VALUE = "valuePerUnit";
        public const string CARGO_ENVIRONMENT = "environmentPerUnit";
        public const string CARGO_LICENSE = "requiredLicense";
        public const string CARGO_SOURCE = "sources";
        public const string CARGO_DEST = "destinations";
    }
}
