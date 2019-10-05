using System;
using System.Collections.Generic;
using Foundation;

namespace ARRunner.Xamarin.SpatialMapping
{
    public interface ISpatialStore
    {
        event EventHandler<EventArgs> StoreChangedEvent; 

        IEnumerable<KeyValuePair<NSUuid, SpatialObject>> KnownObjects { get; }
    }
}
