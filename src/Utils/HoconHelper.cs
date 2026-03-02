using Hocon;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextGameFramework.Utils;

internal static class HoconHelper
{
    private static JsonSerializerOptions CreateJsonOptions()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new UnionJsonConverter());
        DefaultJsonTypeInfoResolver resolver = new();
        resolver.Modifiers.Add(ApplyHoconPropertyNames);
        options.TypeInfoResolver = resolver;
        return options;
    }

    private static void ApplyHoconPropertyNames(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind is not JsonTypeInfoKind.Object)
        {
            return;
        }

        Dictionary<string, string> map = new(StringComparer.Ordinal);
        foreach (PropertyInfo prop in typeInfo.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            HoconPropertyNameAttribute? attr = prop.GetCustomAttribute<HoconPropertyNameAttribute>();
            if (attr is not null)
            {
                map[prop.Name] = attr.Name;
            }
        }

        foreach (FieldInfo field in typeInfo.Type.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            if (field.GetCustomAttribute<JsonIncludeAttribute>() is null)
            {
                continue;
            }

            HoconPropertyNameAttribute? attr = field.GetCustomAttribute<HoconPropertyNameAttribute>();
            if (attr is not null)
            {
                map[field.Name] = attr.Name;
            }
        }

        foreach (JsonPropertyInfo p in typeInfo.Properties)
        {
            if (map.TryGetValue(p.Name, out string? hoconName))
            {
                p.Name = hoconName;
            }
        }
    }

    private static void WriteNodeHocon(Node node, Utf8JsonWriter writer)
    {
        switch (node)
        {
            case ObjectNode obj:
                writer.WriteStartObject();
                foreach (KeyValuePair<string, Node> kv in obj.Children())
                {
                    writer.WritePropertyName(kv.Key);
                    WriteNodeHocon(kv.Value, writer);
                }

                writer.WriteEndObject();
                return;
            case ArrayNode arr:
                writer.WriteStartArray();
                foreach (KeyValuePair<int, Node> kv in arr.Children())
                {
                    WriteNodeHocon(kv.Value, writer);
                }

                writer.WriteEndArray();
                return;
            case ValueNodeHocon vn:
                WriteJsonValueHocon(vn.Value.Value, writer);
                return;
        }
    }

    private static void WriteJsonValueHocon(HoconValue value, Utf8JsonWriter writer)
    {
        switch (value.Type)
        {
            case HoconType.Empty:
                writer.WriteNullValue();
                break;
            case HoconType.Array:
                IList<HoconValue> arr = value.GetArray();
                writer.WriteStartArray();
                foreach (HoconValue v in arr)
                {
                    WriteJsonValueHocon(v, writer);
                }

                writer.WriteEndArray();
                break;
            case HoconType.Object:
                HoconObject obj = value.GetObject();
                writer.WriteStartObject();
                foreach (KeyValuePair<string, HoconField> kv in obj)
                {
                    writer.WritePropertyName(kv.Key);
                    WriteJsonValueHocon(kv.Value.Value, writer);
                }

                writer.WriteEndObject();
                break;
            case HoconType.Number:
                if (value.TryGetLong(out long l))
                {
                    writer.WriteNumberValue(l);
                    break;
                }

                if (value.TryGetDouble(out double d))
                {
                    writer.WriteNumberValue(d);
                    break;
                }

                if (value.TryGetDecimal(out decimal de))
                {
                    writer.WriteNumberValue(de);
                    break;
                }

                string ns = value.GetString();
                writer.WriteStringValue(ns);
                break;
            case HoconType.Boolean:
                bool b = value.GetBoolean();
                writer.WriteBooleanValue(b);
                break;
            case HoconType.String:
                string s = value.GetString();
                writer.WriteStringValue(s);
                break;
        }
    }

    private static void WriteJsonValue(HoconValue value, Utf8JsonWriter writer)
    {
        switch (value.Type)
        {
            case HoconType.Empty:
                writer.WriteNullValue();
                break;
            case HoconType.Array:
                IList<HoconValue> arr = value.GetArray();
                writer.WriteStartArray();
                foreach (HoconValue v in arr)
                {
                    WriteJsonValue(v, writer);
                }

                writer.WriteEndArray();
                break;
            case HoconType.Object:
                HoconObject obj = value.GetObject();
                writer.WriteStartObject();
                foreach (KeyValuePair<string, HoconField> kv in EnumerateObject(obj))
                {
                    writer.WritePropertyName(kv.Key);
                    WriteJsonValue(kv.Value.Value, writer);
                }

                writer.WriteEndObject();
                break;
            case HoconType.Number:
                if (value.TryGetLong(out long l))
                {
                    writer.WriteNumberValue(l);
                    break;
                }

                if (value.TryGetDouble(out double d))
                {
                    writer.WriteNumberValue(d);
                    break;
                }

                if (value.TryGetDecimal(out decimal de))
                {
                    writer.WriteNumberValue(de);
                    break;
                }

                string ns = value.GetString();
                writer.WriteStringValue(ns);
                break;
            case HoconType.Boolean:
                bool b = value.GetBoolean();
                writer.WriteBooleanValue(b);
                break;
            case HoconType.String:
                string s = value.GetString();
                writer.WriteStringValue(s);
                break;
        }
    }

    private static IEnumerable<KeyValuePair<string, HoconField>> EnumerateObject(object obj)
    {
        // 1) IDictionary path
        if (obj is IDictionary dict)
        {
            foreach (DictionaryEntry de in dict)
            {
                if (de.Key is null || de.Value is null)
                {
                    continue;
                }

                if (de.Value is HoconField hv)
                {
                    yield return new(de.Key.ToString()!, hv);
                }
            }

            yield break;
        }

        // 2) IEnumerable of KeyValuePair-like objects with Key/Value properties
        if (obj is IEnumerable enumerable)
        {
            foreach (object entry in enumerable)
            {
                if (entry is null)
                {
                    continue;
                }

                Type entryType = entry.GetType();
                PropertyInfo? keyProp = entryType.GetProperty("Key");
                PropertyInfo? valProp = entryType.GetProperty("Value");
                if (keyProp is null || valProp is null)
                {
                    continue;
                }

                object? keyObj = keyProp.GetValue(entry);
                object? valObj = valProp.GetValue(entry);
                if (keyObj is null || valObj is null)
                {
                    continue;
                }

                if (valObj is HoconField hv)
                {
                    yield return new(keyObj.ToString()!, hv);
                }
            }

            yield break;
        }

        // 3) Fallback: try read "Items" field/property via reflection
        Type t = obj.GetType();
        PropertyInfo? itemsProp =
            t.GetProperty("Items", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (itemsProp is not null)
        {
            object? items = itemsProp.GetValue(obj);
            if (items is IDictionary id2)
            {
                foreach (DictionaryEntry de in id2)
                {
                    if (de.Key is null || de.Value is null)
                    {
                        continue;
                    }

                    if (de.Value is HoconField hv)
                    {
                        yield return new(de.Key.ToString()!, hv);
                    }
                }

                yield break;
            }

            if (items is IEnumerable en2)
            {
                foreach (object entry in en2)
                {
                    if (entry is null)
                    {
                        continue;
                    }

                    Type et = entry.GetType();
                    PropertyInfo? kprop = et.GetProperty("Key");
                    PropertyInfo? vprop = et.GetProperty("Value");
                    if (kprop is null || vprop is null)
                    {
                        continue;
                    }

                    object? key = kprop.GetValue(entry);
                    object? val = vprop.GetValue(entry);
                    if (key is null || val is null)
                    {
                        continue;
                    }

                    if (val is HoconField hv)
                    {
                        yield return new(key.ToString()!, hv);
                    }
                }

                yield break;
            }
        }

        // 4) Try GetKeys() + accessor methods / indexers
        MethodInfo? getKeys = t.GetMethod("GetKeys",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, Type.EmptyTypes,
            null);
        if (getKeys is not null)
        {
            if (getKeys.Invoke(obj, null) is IEnumerable keys)
            {
                // candidate accessors
                MethodInfo? getKey = t.GetMethod("GetKey",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
                    [typeof(string)], null);
                MethodInfo? getValue = t.GetMethod("GetValue",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
                    [typeof(string)], null);
                PropertyInfo? indexer = t.GetProperty("Item",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, [typeof(string)],
                    null);
                foreach (object k in keys)
                {
                    if (k is null)
                    {
                        continue;
                    }

                    string keyStr = k.ToString()!;
                    object? res = null;
                    if (getKey is not null)
                    {
                        res = getKey.Invoke(obj, [keyStr]);
                    }
                    else if (getValue is not null)
                    {
                        res = getValue.Invoke(obj, [keyStr]);
                    }
                    else if (indexer is not null)
                    {
                        res = indexer.GetValue(obj, [keyStr]);
                    }

                    if (res is HoconField hv)
                    {
                        yield return new(keyStr, hv);
                    }
                }
            }
        }
    }

    extension(HoconRoot hocon)
    {
        public T As<T>()
        {
            using MemoryStream ms = new();
            using Utf8JsonWriter writer = new(ms);
            WriteJsonValueHocon(hocon.Value, writer);
            writer.Flush();
            string json = Encoding.UTF8.GetString(ms.ToArray());
            JsonSerializerOptions options = CreateJsonOptions();
            T result = JsonSerializer.Deserialize<T>(json, options) ?? throw new JsonException();
            return result;
        }
    }
}

internal abstract class Node
{
    public ContainerNode? Parent;
    public int? ParentIndex;
    public string? ParentKey;
}

internal abstract class ContainerNode : Node
{
    public abstract void ReplaceSelf(ContainerNode replacement);
}

internal sealed class ObjectNode : ContainerNode
{
    private readonly Dictionary<string, Node> _children = new(StringComparer.Ordinal);

    public override void ReplaceSelf(ContainerNode replacement)
    {
        if (Parent is null)
        {
            return;
        }

        if (Parent is ObjectNode po && ParentKey is not null)
        {
            po._children[ParentKey] = replacement;
        }

        if (Parent is ArrayNode pa && ParentIndex.HasValue)
        {
            pa.Set(ParentIndex.Value, replacement);
        }

        replacement.Parent = Parent;
        replacement.ParentKey = ParentKey;
        replacement.ParentIndex = ParentIndex;
    }

    public void Set(string key, Node node)
    {
        _children[key] = node;
        node.Parent = this;
        node.ParentKey = key;
        node.ParentIndex = null;
    }

    public Node? Get(string key) => _children.TryGetValue(key, out Node? n) ? n : null;
    public IEnumerable<KeyValuePair<string, Node>> Children() => _children;
}

internal sealed class ArrayNode : ContainerNode
{
    private readonly SortedDictionary<int, Node> _children = [];

    public override void ReplaceSelf(ContainerNode replacement)
    {
        if (Parent is null)
        {
            return;
        }

        if (Parent is ObjectNode po && ParentKey is not null)
        {
            po.Set(ParentKey, replacement);
        }

        if (Parent is ArrayNode pa && ParentIndex.HasValue)
        {
            pa.Set(ParentIndex.Value, replacement);
        }

        replacement.Parent = Parent;
        replacement.ParentKey = ParentKey;
        replacement.ParentIndex = ParentIndex;
    }

    public void Set(int idx, Node node)
    {
        _children[idx] = node;
        node.Parent = this;
        node.ParentKey = null;
        node.ParentIndex = idx;
    }

    public Node? Get(int idx) => _children.TryGetValue(idx, out Node? n) ? n : null;
    public IEnumerable<KeyValuePair<int, Node>> Children() => _children;
}

internal sealed class ValueNodeHocon(HoconField v) : Node
{
    public HoconField Value { get; } = v;
}
