using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityModManagerNet;

namespace DVCustomCarLoader
{
    public class CCLSettings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Prefer custom cars to default cars when generating jobs")]
        public bool PreferCustomCarsForJobs = false;

        public void OnChange()
        {
            
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
