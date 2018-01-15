using System;
using System.Collections.Generic;
using Foundation;

namespace ARRunner.Xamarin.SpatialMapping
{
    public class SpatialStore : ISpatialStore
    {
        Dictionary<NSUuid, SpatialObject> _knownObjects = new Dictionary<NSUuid, SpatialObject>();

        public event EventHandler<EventArgs> StoreChangedEvent; 

        public IEnumerable<KeyValuePair<NSUuid, SpatialObject>> KnownObjects { get { return _knownObjects; } }

        public void Register(NSUuid id, SpatialObject spatialObject)
        {
            if(!_knownObjects.ContainsKey(id))
            {
                _knownObjects.Add(id, spatialObject);
                HandleStoreChanged();
                return;
            }

            _knownObjects[id] = spatialObject;
            HandleStoreChanged();
        }

        public void Remove(NSUuid id)
        {
            if (!_knownObjects.ContainsKey(id))
                return;

            _knownObjects.Remove(id);
            HandleStoreChanged();
        }

        private void HandleStoreChanged()
        {
            if (StoreChangedEvent != null)
                StoreChangedEvent(this, EventArgs.Empty);
        }
    }
}
