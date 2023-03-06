using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVCustomCarLoader.LocoComponents
{
    public interface IProvidesDriver
    {
        float reverser { get; }

        float wheelslip { get; }
    }
}
