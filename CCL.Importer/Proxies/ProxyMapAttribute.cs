using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CCL.Importer.Proxies
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =true)]
    internal class ProxyMapAttribute : Attribute
    {
        public Type SourceType { get; private set; }
        public Type DestinationType { get; private set; }
        public IEnumerable<String> FieldsFromCache { get; private set; }
        public string FieldToValidate { get; private set; }
        public dynamic ValidValue { get; private set; }
        public Predicate<MonoBehaviour> Predicate {get; private set;}

        public ProxyMapAttribute(Type sourceType, Type destinationType, string[] fieldsFromCache = null, string fieldToValidate = null, object validValue = null)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
            this.FieldsFromCache = fieldsFromCache ?? new string[] { };
            this.FieldToValidate = fieldToValidate ?? "";
            this.ValidValue = validValue;
            if (FieldToValidate.Length > 0)
            {
                Predicate = source =>
                {
                    if (sourceType.IsInstanceOfType(source))
                    {
                        return sourceType.GetField(FieldToValidate).GetValue(source).Equals(ValidValue);
                    }
                    return false;
                };
            } else
            {
                Predicate = _ => true;
            }
        }
    }
}
