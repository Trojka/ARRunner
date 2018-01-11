using System;
using System.Linq;
using System.Collections.Generic;
using ARRunner.Xamarin.SpatialMapping;
using ARRunner.Xamarin.Util;

namespace ARRunner.Xamarin.SpatialQuerying
{
    public class SpatialQuerying
    {
        Dictionary<int, PlaneQuery> _planeQueries = new Dictionary<int, PlaneQuery>();
        ISpatialStore _store;

        public event EventHandler<EventArgs<(int queryId, SpatialObject o)>> QueryFulfilled;

        public SpatialQuerying(ISpatialStore store)
        {
            _store = store;
            _store.StoreChangedEvent += Store_StoreChangedEvent;
        }

        public int RegisterQuery(PlaneQuery query)
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
            
            foreach(var spatialObject in _store.KnownObjects)
            {
                foreach (var queryEntry in _planeQueries)
                {
                    (var id, var query) = queryEntry;
                    if (query.ObjectFullfillsQuery(spatialObject))
                        QueryFulfilled(this, new EventArgs<(int queryId, SpatialObject o)>((id, spatialObject)));
                }
            
            }
        }

    }
}
