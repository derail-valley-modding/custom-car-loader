using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL_GameScripts
{
    public class CargoModelSetup : MonoBehaviour
    {
        public BaseCargoType CargoType = BaseCargoType.Coal;
        public string CustomCargo = null;

        public GameObject Model = null;
        public string BaseModel = null;

        public override string ToString()
        {
            string cargo = !string.IsNullOrEmpty(CustomCargo) ? CustomCargo : CargoType.ToString();
            string model = !string.IsNullOrEmpty(BaseModel) ? BaseModel : (Model ? Model.name : "none");
            return $"[{cargo}, {model}]";
        }

        public void OnValidate()
        {
            if (!string.IsNullOrEmpty(CustomCargo))
            {
                CargoType = BaseCargoType.Custom;
            }

            if (Model)
            {
                BaseModel = null;
            }
        }
    }
}
