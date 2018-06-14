using System;
using System.Collections;
using System.Collections.Generic;

namespace Xamzor.UI
{
    public class PropertyContainer : IEnumerable<KeyValuePair<PropertyKey, object>>
    {
        private readonly Dictionary<PropertyKey, object> _props =
            new Dictionary<PropertyKey, object>();

        public T Get<T>(PropertyKey property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!typeof(T).IsAssignableFrom(property.ValueType))
            {
                throw new ArgumentException(
                     $"Invalid type specified when getting value of property '{property.Name}'. " +
                     "Expected '{property.ValueType.Name}' or a less derived type, but got '{typeof(T).Name}'.", nameof(T));
            }

            return _props.TryGetValue(property, out var value)
                ? (T)value 
                : (T)property.DefaultValue;
        }

        public void Set(PropertyKey property, object value)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (value == null ||
                (property.ValueType.IsValueType && value == Activator.CreateInstance(property.ValueType)))
            {
                _props.Remove(property);
                return;
            }

            if (!property.ValueType.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException(
                      $"Invalid value specified when setting value of property '{property.Name}'. " +
                      $"Expected value of type '{property.ValueType.Name}' or of a more derived type, but got value of type '{value.GetType().Name}'.", nameof(value));
            }

            _props[property] = value;
        }

        public IEnumerator<KeyValuePair<PropertyKey, object>> GetEnumerator() =>
            ((IEnumerable<KeyValuePair<PropertyKey, object>>)_props).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
