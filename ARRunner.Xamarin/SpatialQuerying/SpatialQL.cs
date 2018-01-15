using System;
using System.Linq;
using System.Collections.Generic;
using ARRunner.Xamarin.SpatialMapping;
using ARRunner.Xamarin.Util;
using Foundation;

namespace ARRunner.Xamarin.SpatialQuerying
{
    public class SpatialQL
    {
        Dictionary<int, Func<Plane, bool>> _planeQueries = new Dictionary<int, Func<Plane, bool>>();
        ISpatialStore _store;

        public event EventHandler<EventArgs<(int queryId, NSUuid spatialObjectId)>> QueryFulfilled;

        public SpatialQL(ISpatialStore store)
        {
            _store = store;
            _store.StoreChangedEvent += Store_StoreChangedEvent;
        }

        public int RegisterQuery(Func<Plane, bool> query)
        {
            int id = 0;
            if(_planeQueries.Keys.Count() > 0)
            {
                id = _planeQueries.Keys.Max();
            }
            id++;
            _planeQueries.Add(id, query);

            return id;
        }

        void Store_StoreChangedEvent(object sender, EventArgs e)
        {
            if (QueryFulfilled == null)
                return;
            
            foreach(var spatialObjectEntry in _store.KnownObjects)
            {
                foreach (var queryEntry in _planeQueries)
                {
                    (var id, var query) = queryEntry;
                    (var objectId, var spatialObject) = spatialObjectEntry;
                    if (spatialObject is Plane && query(spatialObject as Plane))
                        QueryFulfilled(this, new EventArgs<(int queryId, NSUuid spatialObjectId)>((id, objectId)));
                }
            
            }
        }

    }
}
