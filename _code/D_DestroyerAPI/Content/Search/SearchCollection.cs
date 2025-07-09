using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AICPA.Destroyer.Content.Search
{
    public class SearchCollection : ICollection<ISearchCriteria>
    {
        private List<ISearchCriteria> searches = new List<ISearchCriteria>();
        
        public SearchCollection()
        {
        }
        
        public SearchCollection(IEnumerable<ISearchCriteria> searches)
        {
            this.searches.AddRange(searches);
        }

        public ISearchCriteria this[string savedSearchName]
        {
            get
            {
                return searches.Where(search => search.Name == savedSearchName).SingleOrDefault();
            }
        }

        public ISearchCriteria this[int savedSearchId]
        {
            get
            {
                return searches.Where(search => search.SaveId == savedSearchId).SingleOrDefault();
            }
        }

        public bool Contains(string savedSearchName)
        {
            return this[savedSearchName] != null;
        }

        public bool Contains(int savedSearchId)
        {
            return this[savedSearchId] != null;
        }

        #region ICollection<ISearchCriteria> Members

        [Obsolete("Use other Add method because this collection only supports adding SearchCriteria objects that have not been persisted yet.", true)]
        public void Add(ISearchCriteria item)
        {
            throw new NotSupportedException("Use other Add method because this collection only supports adding SearchCriteria objects that have not been persisted yet.");
        }

        public void Add(ISearchCriteria item, string savedSearchName, Guid userId)
        {
            item.Save(savedSearchName, userId);
            searches.Add(item);
        }

        public void Clear()
        {
            foreach (ISearchCriteria search in searches)
            {
                SearchCriteria.DeleteSearchById(search.SaveId);
            }

            searches.Clear();
        }

        public bool Contains(ISearchCriteria item)
        {
            return searches.Contains(item);
        }

        public void CopyTo(ISearchCriteria[] array, int arrayIndex)
        {
            searches.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return searches.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(int savedId)
        {
            bool success = false;
            ISearchCriteria item = this[savedId];

            if (Contains(item))
            {
                SearchCriteria.DeleteSearchById(item.SaveId);
                success = searches.Remove(item);
            }

            return success;
        }

        public bool Remove(string savedName)
        {
            bool success = false;
            ISearchCriteria item = this[savedName];

            if (Contains(item))
            {
                SearchCriteria.DeleteSearchById(item.SaveId);
                success = searches.Remove(item);
            }

            return success;
        }

        public bool Remove(ISearchCriteria item)
        {
            bool success = false;

            if (Contains(item))
            {
                SearchCriteria.DeleteSearchById(item.SaveId);
                success = searches.Remove(item);
            }

            return success;
        }

        #endregion

        #region IEnumerable<ISearchCriteria> Members

        public IEnumerator<ISearchCriteria> GetEnumerator()
        {
            return searches.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return searches.GetEnumerator();
        }

        #endregion
    }
}
