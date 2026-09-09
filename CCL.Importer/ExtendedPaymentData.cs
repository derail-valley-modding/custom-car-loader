using DV.ThingTypes;
using System.Collections.Generic;
using System.Linq;

namespace CCL.Importer
{
    public class ExtendedPaymentData : PaymentCalculationData
    {
        public class TypeCargoData
        {
            public TrainCarType_v2 CarType;
            public CargoType Cargo;
            public int Count;

            public TypeCargoData(StationProceduralJobGenerator.CarTypesPerCargoTypeData data)
            {
                CarType = data.carTypes.First().parentType;
                Cargo = data.cargoType;
                Count = data.carTypes.Count;
            }

            public TypeCargoData(TrainCarType_v2 carType, CargoType cargo, int count)
            {
                CarType = carType;
                Cargo = cargo;
                Count = count;
            }
        }

        public List<TypeCargoData> ExtendedData;

        public ExtendedPaymentData(Dictionary<TrainCarLivery, int> carsData, Dictionary<CargoType, int> cargoData)
            : base(carsData, cargoData)
        {
            ExtendedData = new List<TypeCargoData>();
        }

        public static ExtendedPaymentData FromRegular(PaymentCalculationData original, List<StationProceduralJobGenerator.CarTypesPerCargoTypeData> carTypesPerCargoData)
        {
            var extended = new ExtendedPaymentData(original.carsData, original.cargoData);

            if (carTypesPerCargoData == null) return extended;

            foreach (var item in carTypesPerCargoData)
            {
                extended.ExtendedData.Add(new TypeCargoData(item));
            }

            return extended;
        }

        public static ExtendedPaymentData ForSingleCargoType(PaymentCalculationData original, List<TrainCarLivery> carTypes, CargoType cargoType)
        {
            var extended = new ExtendedPaymentData(original.carsData, original.cargoData);
            var dict = new Dictionary<TrainCarType_v2, int>();

            foreach (var livery in carTypes)
            {
                if (!dict.ContainsKey(livery.parentType))
                {
                    dict[livery.parentType] = 0;
                }

                dict[livery.parentType]++;
            }

            foreach (var item in dict)
            {
                extended.ExtendedData.Add(new TypeCargoData(item.Key, cargoType, item.Value));
            }

            return extended;
        }
    }
}
