using System;
using System.Collections.Generic;

using System.Reflection;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Xml;
using System.Xml.Serialization;

using System.Linq.Expressions;

namespace CloneDto
{
    public class CloneUtility
    {
        #region MemberwiseClone
        public static object MemberwiseClone(object dto)
        {
            // Do a shallow copy via objects protected method MemberwiseClone.
            var mi = dto.GetType().GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
            var newDto = mi.Invoke(dto, new object[] { });
            return newDto;
        }
        #endregion

        #region Serializable - BinaryFormatter
        /// <summary>
        /// Perform a deep Copy of the object.  Must be serializable.
        /// http://stackoverflow.com/questions/78536/deep-cloning-objects-in-c-sharp
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T SerializableClone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        #endregion

        #region Serializable - XmlSerializer
        /// <summary>
        /// Watch out for "memory leak".
        /// http://stackoverflow.com/questions/23897145/memory-leak-using-streamreader-and-xmlserializer
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="obj">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T XmlSerializerClone<T>(T obj)
        {
            // MSDN code with minor changes
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(XmlWriter.Create(stream), obj);
                // stream.Flush();  MemoryStream overrides Flush and takes no action, so no need to call.
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(XmlReader.Create(stream));
            }
        }
        #endregion

        #region Expression Tree
        /// <summary>
        /// Great a compiled lamba expression.
        /// Once created this approach is very fast.
        /// </summary>
        /// <param name="type">The type of object being copied.</param>
        /// <returns>A compiled expression tree that will copy a dto.</returns>
        public static Func<object, object> CreateCloneDelegate(Type type)
        {
            // FYI: Could set properties on creating new DTO.
            // Used calling of Getters and Setters seeing more likely to be used in other scenarios.

            // Input parameter
            var value = Expression.Parameter(typeof(object), "value");

            // Input parameter cast to type
            var typedValue = Expression.Convert(value, type);

            // Result parameter         
            var result = Expression.Parameter(type, "result");          

            var exps = new List<Expression>();

            // Create new object
            exps.Add(Expression.Assign(result, Expression.New(type)));  
            foreach (var pi in type.GetProperties())
            {
                // Assign property.
                exps.Add(Expression.Call(result, pi.GetSetMethod(), Expression.Call(typedValue, pi.GetGetMethod())));
            }

            // Last expression in block is returned value
            exps.Add(result);

            // First parameter of block method for variables
            var block = Expression.Block(new[] { result }, exps);

            // Compile expression and return a delegate
            return Expression.Lambda<Func<object, object>>(block, value).Compile(); 
        }
        #endregion

        #region Reflection Deep Copy
        /// <summary> 
        /// Get the deep clone of an object. 
        /// http://code.msdn.microsoft.com/windowsdesktop/CSDeepCloneObject-8a53311e/view/SourceCode#content
        /// </summary> 
        /// <typeparam name="T">The type of the obj.</typeparam> 
        /// <param name="obj">It is the object used to deep clone.</param> 
        /// <returns>Return the deep clone.</returns> 
        public static T ReflectionDeepClone<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            return (T)CloneProcedure(obj);
        }

        private static object CloneProcedure(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var type = obj.GetType();

            // If the type of object is the value type, we will always get a new object when  
            // the original object is assigned to another variable. So if the type of the  
            // object is primitive or enum, we just return the object. We will process the  
            // struct type subsequently because the struct type may contain the reference  
            // fields.
            // If the string variables contain the same chars, they always refer to the same  
            // string in the heap. So if the type of the object is string, we also return the  
            // object. 
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return obj;
            }

            // If the type of the object is the Array, we use the CreateInstance method to get 
            // a new instance of the array. We also process recursively this method on the  
            // elements of the original array because the type of the element may be a reference  
            // type. 
            if (type.IsArray)
            {
                var typeElement = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = (Array)obj; 
                var copiedArray = Array.CreateInstance(typeElement, array.Length);
                for (var i = 0; i < array.Length; i++)
                {
                    // Get the deep clone of the element in the original array and assign the  
                    // clone to the new array. 
                    copiedArray.SetValue(CloneProcedure(array.GetValue(i)), i);

                }
                return copiedArray;
            }

            // If the type of the object is class or struct, it may contain the reference fields,  
            // so we use reflection and process recursively this method in the fields of the object  
            // to get the deep clone of the object.  
            // We use Type.IsValueType method here because there is no way to indicate directly whether  
            // the Type is a struct type. 
            if (type.IsClass || type.IsValueType)
            {
                var copiedObject = Activator.CreateInstance(obj.GetType());

                // Get all FieldInfo. 
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(obj);
                    if (fieldValue != null)
                    {
                        // Get the deep clone of the field in the original object and assign  
                        // the clone to the field in the new object. 
                        field.SetValue(copiedObject, CloneProcedure(fieldValue));
                    }

                }
                return copiedObject;
            }

            throw new ArgumentException("The object is an unknown type");
        }
        #endregion
    }
}