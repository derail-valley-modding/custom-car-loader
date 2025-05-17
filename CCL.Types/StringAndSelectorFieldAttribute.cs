using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CCL.Types
{
    public class StringAndSelectorFieldAttribute : PropertyAttribute
    {
        public List<string> Options;
        public bool CustomAllowed;

        public StringAndSelectorFieldAttribute(IEnumerable<string> options, bool customAllowed)
        {
            Options = new List<string> { "Not Set" };
            Options.AddRange(options);

            if (customAllowed)
            {
                Options.Add("CUSTOM");
            }

            CustomAllowed = customAllowed;
        }

        public StringAndSelectorFieldAttribute(string[] optionsArray, bool customAllowed) : this(options: optionsArray, customAllowed) { }
    }

    public class CargoFieldAttribute : StringAndSelectorFieldAttribute
    {
        public CargoFieldAttribute(bool customAllowed = true) : base(IdV2.Cargos, customAllowed) { }
    }

    public class GeneralLicenseFieldAttribute : StringAndSelectorFieldAttribute
    {
        public GeneralLicenseFieldAttribute(bool customAllowed = true) : base(IdV2.GeneralLicenses, customAllowed) { }
    }

    public class JobLicenseFieldAttribute : StringAndSelectorFieldAttribute
    {
        public JobLicenseFieldAttribute(bool customAllowed = true) : base(IdV2.JobLicenses, customAllowed) { }
    }

    public class AnyLicenseFieldAttribute : StringAndSelectorFieldAttribute
    {
        public AnyLicenseFieldAttribute(bool customAllowed = true) : base(IdV2.GeneralLicenses.Concat(IdV2.JobLicenses), customAllowed) { }
    }

    public class PaintFieldAttribute : StringAndSelectorFieldAttribute
    {
        public PaintFieldAttribute(bool customAllowed = true) : base(IdV2.Paints, customAllowed) { }
    }

    public class CarKindFieldAttribute : StringAndSelectorFieldAttribute
    {
        public CarKindFieldAttribute() : base(IdV2.CarKinds, false) { }
    }

    public class ControlActionFieldAttribute : StringAndSelectorFieldAttribute
    {
        public ControlActionFieldAttribute() : base(ControlActions.Names, false) { }
    }
}
