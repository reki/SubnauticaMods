using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AshFox.Subnautica
{
    public static class LocalizationManager
    {
        private const string FallbackLanguage = "english";
        private static Dictionary<string, JObject> _localizations; // language -> root JObject
        private static JObject _defaultLocalizations; // language -> root JObject
        private static JObject _currentLocalizations;

        public static void Initialize()
        {
            _localizations = new Dictionary<string, JObject>();
            LoadLocalizations();
        }

        private static void LoadLocalizations()
        {
            try
            {
                var callingAssembly = typeof(LocalizationManager).Assembly;
                string pluginPath = Path.GetDirectoryName(callingAssembly.Location);

                var candidates = new List<string>();
                if (!string.IsNullOrEmpty(pluginPath))
                {
                    candidates.Add(Path.Combine(pluginPath, "Localization"));
                    var parent = Directory.GetParent(pluginPath)?.FullName;
                    if (!string.IsNullOrEmpty(parent))
                        candidates.Add(Path.Combine(parent, "Localization"));
                }

                string localizationPath = null;
                foreach (var c in candidates)
                {
                    if (Directory.Exists(c))
                    {
                        localizationPath = c;
                        break;
                    }
                }

                if (localizationPath == null)
                {
                    Debug.LogWarning(
                        $"[Localization] Localization directory not found in candidates: {string.Join(", ", candidates)}"
                    );
                    return;
                }

                foreach (
                    var file in Directory.GetFiles(
                        localizationPath,
                        "*.json",
                        SearchOption.TopDirectoryOnly
                    )
                )
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    if (string.IsNullOrWhiteSpace(name))
                        continue;

                    string jsonContent = File.ReadAllText(file, Encoding.UTF8);
                    var root = JsonConvert.DeserializeObject<JObject>(jsonContent);
                    if (root != null)
                    {
                        _localizations[name.ToLower()] = root; // 言語キーはそのまま格納（変換しない）
                    }
                }
                _defaultLocalizations = _localizations[FallbackLanguage.ToLower()];
                _currentLocalizations = _defaultLocalizations;
                SetLanguage();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Localization] Failed to load localizations: {ex.Message}");
            }
        }

        private static void SetLanguage()
        {
            string gameLanguage =
                Language.main?.GetCurrentLanguage().ToLower() ?? FallbackLanguage.ToLower();
            // そのまま一致を最優先
            if (_localizations.ContainsKey(gameLanguage))
            {
                _currentLocalizations = _localizations[gameLanguage];
            }
            else
            {
                _currentLocalizations = _defaultLocalizations;
            }
        }

        public static string GetLocalizedString(
            string key,
            Dictionary<string, string> variables = null
        )
        {
            if (_localizations == null)
            {
                Plugin.Log.LogWarning($"[Localization] Localizations are not loaded");
                return key;
            }

            string value = ResolveHierarchicalKey(_currentLocalizations, key);
            if (value != null)
            {
                return WrapVariables(value, variables);
            }
            value = ResolveHierarchicalKey(_defaultLocalizations, key);
            if (value != null)
            {
                return WrapVariables(value, variables);
            }
            return key;
        }

        private static string WrapVariables(string value, Dictionary<string, string> variables)
        {
            if (variables == null)
                return value;
            foreach (var kv in variables)
                value = value.Replace($"{{{kv.Key}}}", kv.Value);
            return value;
        }

        private static string ResolveHierarchicalKey(JObject root, string key)
        {
            var parts = key.Split('.');
            JToken current = root;
            for (int i = 0; i < parts.Length; i++)
            {
                if (current is JObject obj)
                {
                    if (!obj.TryGetValue(parts[i], out current))
                    {
                        Plugin.Log.LogWarning(
                            $"[Localization] Failed to resolve hierarchical key: {key}"
                        );
                        return null;
                    }
                }
                else
                {
                    Plugin.Log.LogWarning(
                        $"[Localization] Current token is not an object for key: {key}"
                    );
                    return null;
                }
            }
            if (current is JValue val && val.Type == JTokenType.String)
                return val.ToString();

            return null;
        }
    }
}
