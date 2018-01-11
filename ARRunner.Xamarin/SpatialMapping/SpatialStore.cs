using System;
using System.Collections.Generic;

namespace ARRunner.Xamarin.SpatialMapping
{
    public class SpatialStore<T> : ISpatialStore
    {
        Dictionary<T, SpatialObject> _knownObjects = new Dictionary<T, SpatialObject>();

        public event EventHandler<EventArgs> StoreChangedEvent; 

        public IEnumerable<SpatialObject> KnownObjects { get { return _knownObjects.Values; } }

        public void Register(T id, SpatialObject spatialObject)
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

        public void Remove(T id)
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
