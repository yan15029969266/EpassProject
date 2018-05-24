using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Interface
{
    public interface IPcomm:IDisposable
    {
        IPcomm LinkToScreen<IScreen>(Predicate<IScreen> action) where IScreen : new();

        IPcomm SkipToHomeScreen<IScreen>() where IScreen : new();
    }
}
