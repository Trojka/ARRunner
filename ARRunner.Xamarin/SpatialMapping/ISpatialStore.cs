using System;
using System.Collections.Generic;

namespace ARRunner.Xamarin.SpatialMapping
{
    public interface ISpatialStore
    {
        event EventHandler<EventArgs> StoreChangedEvent; 

        IEnumerable<SpatialObject> KnownObjects { get; }
    }
}
