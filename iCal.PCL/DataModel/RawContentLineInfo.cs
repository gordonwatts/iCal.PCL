using System.Collections.Generic;

namespace iCal.PCL.DataModel
{
    /// <summary>
    /// Contains the data that appears after a parameter name on a iCal content line
    /// </summary>
    public class RawContentLineInfo
    {
        /// <summary>
        /// Hold onto any parameter names
        /// </summary>
        private Dictionary<string, string> _parameters = null;

        /// <summary>
        /// Return the value of a parameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (_parameters == null)
                    throw new KeyNotFoundException(key);
                return _parameters[key];
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
        public void Add(string key, string value)
        {
            if (_parameters == null)
            {
                _parameters = new Dictionary<string, string>();
            }
            _parameters[key] = value;
        }
    }
}
