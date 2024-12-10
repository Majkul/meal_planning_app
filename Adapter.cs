using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Adapter{
    public interface ISerializableAdapter {
    string Serialize();
    }

    public class JsonAdapter<T> : ISerializableAdapter {
        private readonly T _object;

        public JsonAdapter(T obj) {
            _object = obj;
        }

        public string Serialize() {
            return JsonSerializer.Serialize(_object);
        }
    }

    public class XmlAdapter<T> : ISerializableAdapter {
        private readonly T _object;

        public XmlAdapter(T obj) {
            _object = obj;
        }

        public string Serialize() {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter()) {
                serializer.Serialize(writer, _object);
                return writer.ToString();
            }
        }
    }


    public class TxtAdapter<T> : ISerializableAdapter {
        private readonly T _object;

        public TxtAdapter(T obj) {
            _object = obj;
        }

        public string Serialize() {
            var sb = new StringBuilder();
            foreach (var prop in typeof(T).GetProperties()) {
                sb.AppendLine($"{prop.Name}: {prop.GetValue(_object)}");
            }
            return sb.ToString();
        }
    }
}