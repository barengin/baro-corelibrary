using System;
using System.Collections.Generic;
using System.Text;

namespace Baro.CoreLibrary.Configuration
{
    public interface IConfigSection: IEnumerable<KeyValuePair<string, string>>
    {
        string Name { get; }
        void Clear();

        string AsString(string key);
        int AsInt32(string key);
        long AsLong(string key);
        double AsDouble(string key);
        bool AsBool(string key);

        string AsString(string key, string defaultValue, bool addIfNotExists);
        int AsInt32(string key, Int32 defaultValue, bool addIfNotExists);
        long AsLong(string key, long defaultValue, bool addIfNotExists);
        double AsDouble(string key, double defaultValue, bool addIfNotExists);
        bool AsBool(string key, bool defaultValue, bool addIfNotExists);

        void AsString(string key, string value);
        void AsInt32(string key, int value);
        void AsLong(string key, long value);
        void AsDouble(string key, double value);
        void AsBool(string key, bool value);

        void RemoveKey(string key);
        bool ContainsKey(string key);

        string this[string key] { get; }
    }
}
