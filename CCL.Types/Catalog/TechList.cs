using System;
using System.Collections.Generic;

namespace CCL.Types.Catalog
{
    [Serializable]
    public class TechList
    {
        public TechEntry Tech1 = new TechEntry();
        public TechEntry Tech2 = new TechEntry();
        public TechEntry Tech3 = new TechEntry();
        public TechEntry Tech4 = new TechEntry();
        public TechEntry Tech5 = new TechEntry();
        public TechEntry Tech6 = new TechEntry();
        public TechEntry Tech7 = new TechEntry();

        public IEnumerable<TechEntry> AllTechs
        {
            get
            {
                yield return Tech1;
                yield return Tech2;
                yield return Tech3;
                yield return Tech4;
                yield return Tech5;
                yield return Tech6;
                yield return Tech7;
            }
        }
    }
}
