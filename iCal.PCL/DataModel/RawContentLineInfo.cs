using System.Collections.Generic;

namespace iCal.PCL.DataModel
{
    /// <summary>
    /// Contains the data that appears after a parameter name on a iCal content line
    /// when parsing a complex (parameter possible) line.
    /// </summary>
    public class RawContentLineInfo : IDictionary<string, string[]>
    {
        /// <summary>
        /// Hold onto any parameter names
        /// </summary>
        private Dictionary<string, string[]> _parameters = null;

        /// <summary>
        /// Return the value of a parameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] this[string key]
        {
            get
            {
                if (_parameters == null)
                    throw new KeyNotFoundException(key);
                return _parameters[key];
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Add a new parameter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, string[] value)
        {
            if (_parameters == null)
            {
                _parameters = new Dictionary<string, string[]>();
            }
            _parameters[key] = value;
        }


        public bool ContainsKey(string key)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<string> Keys
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool Remove(string key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(string key, out string[] value)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<string[]> Values
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Add(KeyValuePair<string, string[]> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, string[]> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string[]> item)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
