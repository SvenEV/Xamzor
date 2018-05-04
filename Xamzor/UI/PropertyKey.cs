using System;
using System.Collections.Generic;

namespace Xamzor.UI
{
    public class PropertyKey : IEquatable<PropertyKey>
    {
        public string Name { get; }

        public Type ValueType { get; }

        public Type OwnerType { get; }

        public object DefaultValue { get; }

        public PropertyKey(string name, Type valueType, Type ownerType, object defaultValue)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
            DefaultValue = defaultValue;
        }

        public static PropertyKey Create<TValue, TOwner>(string name, TValue defaultValue = default(TValue)) =>
            new PropertyKey(name, typeof(TValue), typeof(TOwner), defaultValue);

        public override bool Equals(object obj)
        {
            return Equals(obj as PropertyKey);
        }

        public bool Equals(PropertyKey other)
        {
            return other != null &&
                   EqualityComparer<Type>.Default.Equals(OwnerType, other.OwnerType) &&
                   EqualityComparer<Type>.Default.Equals(ValueType, other.ValueType) &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -1025461572;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(OwnerType);
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ValueType);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public static bool operator ==(PropertyKey a, PropertyKey b) =>
            EqualityComparer<PropertyKey>.Default.Equals(a, b);

        public static bool operator !=(PropertyKey property1, PropertyKey property2) =>
            !(property1 == property2);

        public override string ToString() =>
            $"{OwnerType.Name}.{Name} : {ValueType.Name} = {DefaultValue ?? "null"}";
    }
}
