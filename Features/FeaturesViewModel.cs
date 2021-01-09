using System.Collections.Generic;
using System.Reflection;

namespace FenixAlliance.ABP.Hub.Features
{
    public class FeaturesViewModel
    {
        public List<TypeInfo> Controllers { get; set; }

        public List<TypeInfo> TagHelpers { get; set; }

        public List<TypeInfo> ViewComponents { get; set; }
    }
}