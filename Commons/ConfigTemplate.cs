using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AshFox.Subnautica
{
    public class ConfigTemplate
    {
        private JObject _config;

        public ConfigTemplate(string filePath)
        {
            LoadConfig(filePath);
        }

        public bool LoadConfig(string filePath)
        {
            string modDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string fullPath = Path.Combine(modDirectory, filePath);
            if (!File.Exists(fullPath))
            {
                _config = null;
                return false;
            }
            _config = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(fullPath));
            return true;
        }

        private JValue getValue(string key)
        {
            if (_config == null)
            {
                return null;
            }
            var parts = key.Split('.');
            JToken current = _config;
            for (int i = 0; i < parts.Length; i++)
            {
                if (current is JObject obj)
                {
                    if (!obj.TryGetValue(parts[i], out current))
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            if (current is JValue val)
            {
                return val;
            }
            return null;
        }

        public String GetString(string key, string defaultValue = null)
        {
            return GetString(key, () => defaultValue);
        }

        public String GetString(string key, Func<string> defaultValue)
        {
            if (_config == null)
            {
                return defaultValue();
            }

            var value = getValue(key);
            if (value == null || value.Type != JTokenType.String)
            {
                return defaultValue();
            }
            return value.ToString();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return GetInt(key, () => defaultValue);
        }

        public int GetInt(string key, Func<int> defaultValue)
        {
            if (_config == null)
            {
                return defaultValue();
            }
            var value = getValue(key);
            if (value == null || value.Type != JTokenType.Integer)
            {
                return defaultValue();
            }
            return value.Value<int>();
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            return GetFloat(key, () => defaultValue);
        }

        public float GetFloat(string key, Func<float> defaultValue)
        {
            if (_config == null)
            {
                return defaultValue();
            }
            var value = getValue(key);
            if (value == null || value.Type != JTokenType.Float)
            {
                return defaultValue();
            }
            return value.Value<float>();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            return GetBool(key, () => defaultValue);
        }

        public bool GetBool(string key, Func<bool> defaultValue)
        {
            if (_config == null)
            {
                return defaultValue();
            }
            var value = getValue(key);
            if (value == null || value.Type != JTokenType.Boolean)
            {
                return defaultValue();
            }
            return value.Value<bool>();
        }
    }
}
