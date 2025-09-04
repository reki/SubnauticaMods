using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace AshFox.Subnautica
{
    public class VehicleTeleporterManager : MonoBehaviour
    {
        // ===== Warper SFX cache =====
        private static bool _warperSfxInitialized = false;
        private static bool _warperSfxLoading = false;
        private static List<string> _warperOutEventPaths = new List<string>();
        private static List<string> _warperInEventPaths = new List<string>();

        // private static GameObject _dialogPrefab; // 未使用のためコメントアウト
        private static GameObject _currentDialog;
        private static bool _isSearchingVehicles = false;

        // 新しい選択システム用の変数
        private static List<VehicleInfo> _availableVehicles = new List<VehicleInfo>();
        private static int _selectedVehicleIndex = -1;

        // 新しい選択システム: 右クリックで次の乗り物を選択
        public static void SelectNextVehicle()
        {
            // 初回呼び出し時にワーパー音を初期化
            if (!_warperSfxInitialized && !_warperSfxLoading)
            {
                _warperSfxLoading = true;
                CoroutineHost.StartCoroutine(InitWarperSfxAsync());
            }

            _availableVehicles = GetAvailableVehicles();

            if (_availableVehicles.Count == 0)
            {
                ShowMessage(
                    LocalizationManager.GetLocalizedString(
                        "VehicleTeleporterMessages.NoVehiclesFound"
                    )
                );
                return;
            }

            // 次の乗り物を選択
            _selectedVehicleIndex = (_selectedVehicleIndex + 1) % _availableVehicles.Count;
            var selectedVehicle = _availableVehicles[_selectedVehicleIndex];

            ShowMessage(
                LocalizationManager.GetLocalizedString(
                    "VehicleTeleporterMessages.VehicleSelected",
                    new Dictionary<string, string> { { "name", selectedVehicle.Name } }
                )
            );
        }

        // 新しい選択システム: 左クリックで選択中の乗り物をテレポート
        public static void TeleportSelectedVehicle()
        {
            if (_selectedVehicleIndex < 0 || _selectedVehicleIndex >= _availableVehicles.Count)
            {
                ShowMessage(
                    LocalizationManager.GetLocalizedString(
                        "VehicleTeleporterMessages.NoVehicleSelected"
                    )
                );
                return;
            }

            var selectedVehicle = _availableVehicles[_selectedVehicleIndex];

            TeleportVehicle(selectedVehicle);
        }

        public static void ShowTeleportDialog()
        {
            // このメソッドは後方互換性のため残すが、新しいシステムでは使用しない
        }

        private static List<VehicleInfo> GetAvailableVehicles()
        {
            var vehicles = new List<VehicleInfo>();

            // 無限ループを防ぐためのチェック
            if (_isSearchingVehicles)
            {
                Plugin.Log.LogWarning(
                    "GetAvailableVehicles already running, skipping to prevent infinite loop"
                );
                return vehicles;
            }
            _isSearchingVehicles = true;

            // ワールドに展開されている乗り物を検索
            var seamoths = FindObjectsOfType<SeaMoth>();

            foreach (var seamoth in seamoths)
            {
                if (seamoth != null && !seamoth.docked)
                {
                    vehicles.Add(
                        new VehicleInfo
                        {
                            TechType = TechType.Seamoth,
                            Name = GetRegisteredVehicleName(seamoth.gameObject, TechType.Seamoth),
                            IsStored = false,
                            VehicleObject = seamoth.gameObject,
                        }
                    );
                }
            }

            var prawnSuits = FindObjectsOfType<Exosuit>();
            foreach (var prawnSuit in prawnSuits)
            {
                if (prawnSuit != null && !prawnSuit.docked)
                {
                    vehicles.Add(
                        new VehicleInfo
                        {
                            TechType = TechType.Exosuit,
                            Name = GetRegisteredVehicleName(prawnSuit.gameObject, TechType.Exosuit),
                            IsStored = false,
                            VehicleObject = prawnSuit.gameObject,
                        }
                    );
                }
            }

            _isSearchingVehicles = false;
            return vehicles;
        }

        private static string GetVehicleDisplayName(TechType techType)
        {
            switch (techType)
            {
                case TechType.Seamoth:
                    return "Seamoth";
                case TechType.Exosuit:
                    return "Prawn Suit";
                default:
                    return techType.AsString();
            }
        }

        // 可能ならプレイヤーが付けた登録名（カスタム名）を返す。無ければ型名にフォールバック。
        private static string GetRegisteredVehicleName(GameObject vehicleObject, TechType techType)
        {
            if (vehicleObject == null)
                return GetVehicleDisplayName(techType);

            // 1) PingInstance 由来のカスタムラベルを反射で取得
            try
            {
                var pingType = Type.GetType("PingInstance, Assembly-CSharp", false);
                if (pingType != null)
                {
                    var ping = vehicleObject.GetComponent(pingType);
                    if (ping != null)
                    {
                        // 候補: customLabel / label / GetCustomLabel() / GetLabel()
                        var candidates = new (string prop, string method)[]
                        {
                            ("customLabel", null),
                            ("label", null),
                            (null, "GetCustomLabel"),
                            (null, "GetLabel"),
                        };
                        foreach (var (propName, methodName) in candidates)
                        {
                            string val = null;
                            if (!string.IsNullOrEmpty(propName))
                            {
                                var prop = pingType.GetProperty(
                                    propName,
                                    BindingFlags.Public
                                        | BindingFlags.NonPublic
                                        | BindingFlags.Instance
                                );
                                if (prop != null && prop.PropertyType == typeof(string))
                                {
                                    val = prop.GetValue(ping) as string;
                                }
                                if (val == null)
                                {
                                    var field = pingType.GetField(
                                        propName,
                                        BindingFlags.Public
                                            | BindingFlags.NonPublic
                                            | BindingFlags.Instance
                                    );
                                    if (field != null && field.FieldType == typeof(string))
                                        val = field.GetValue(ping) as string;
                                }

                                if (!string.IsNullOrEmpty(val))
                                    return val;
                            }

                            if (!string.IsNullOrEmpty(methodName))
                            {
                                var method = pingType.GetMethod(
                                    methodName,
                                    BindingFlags.Public
                                        | BindingFlags.NonPublic
                                        | BindingFlags.Instance,
                                    null,
                                    Type.EmptyTypes,
                                    null
                                );
                                if (method != null && method.ReturnType == typeof(string))
                                {
                                    var res = method.Invoke(ping, null) as string;
                                    if (!string.IsNullOrEmpty(res))
                                        return res;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            // 2) SubName（Cyclops等）互換: 反射で name/GetName()
            try
            {
                var subNameType = Type.GetType("SubName, Assembly-CSharp", false);
                if (subNameType != null)
                {
                    var subName = vehicleObject.GetComponent(subNameType);
                    if (subName != null)
                    {
                        var prop = subNameType.GetProperty(
                            "name",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        );
                        if (prop != null && prop.PropertyType == typeof(string))
                        {
                            var val = prop.GetValue(subName) as string;
                            if (!string.IsNullOrEmpty(val))
                                return val;
                        }
                        var field = subNameType.GetField(
                            "name",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        );
                        if (field != null && field.FieldType == typeof(string))
                        {
                            var val = field.GetValue(subName) as string;
                            if (!string.IsNullOrEmpty(val))
                                return val;
                        }
                        var method = subNameType.GetMethod(
                            "GetName",
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        );
                        if (method != null && method.ReturnType == typeof(string))
                        {
                            var val = method.Invoke(subName, null) as string;
                            if (!string.IsNullOrEmpty(val))
                                return val;
                        }
                    }
                }
            }
            catch { }

            // フォールバック: 既存の型名
            return GetVehicleDisplayName(techType);
        }

        private static void CreateSelectionDialog(List<VehicleInfo> vehicles)
        {
            // ダイアログのプレハブを作成
            _currentDialog = new GameObject("VehicleTeleporterDialog");
            _currentDialog.transform.SetParent(uGUI.main.transform, false);

            var canvas = _currentDialog.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            var canvasScaler = _currentDialog.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            var raycaster = _currentDialog.AddComponent<GraphicRaycaster>();

            // 背景パネル
            var backgroundPanel = CreatePanel(_currentDialog, "Background");
            var backgroundRect = backgroundPanel.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;

            var backgroundImage = backgroundPanel.GetComponent<Image>();
            backgroundImage.color = new Color(0, 0, 0, 0.7f);

            // 背景クリックでダイアログを閉じる
            var backgroundButton = backgroundPanel.AddComponent<Button>();
            backgroundButton.onClick.AddListener(() =>
            {
                CloseDialog();
            });

            // メインパネル
            var mainPanel = CreatePanel(backgroundPanel, "MainPanel");
            var mainRect = mainPanel.GetComponent<RectTransform>();
            mainRect.anchorMin = new Vector2(0.3f, 0.3f);
            mainRect.anchorMax = new Vector2(0.7f, 0.7f);
            mainRect.offsetMin = Vector2.zero;
            mainRect.offsetMax = Vector2.zero;

            var mainImage = mainPanel.GetComponent<Image>();
            mainImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // メインパネルでイベントの伝播を止める
            var mainButton = mainPanel.AddComponent<Button>();
            mainButton.onClick.AddListener(() => { }); // 何もしない

            // タイトル
            var titleText = CreateText(
                mainPanel,
                "Title",
                LocalizationManager.GetLocalizedString("VehicleTeleporterDialog.Title")
            );
            var titleRect = titleText.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(10, 0);
            titleRect.offsetMax = new Vector2(-10, -10);

            var titleTextComponent = titleText.GetComponent<Text>();
            titleTextComponent.fontSize = 24;
            titleTextComponent.fontStyle = FontStyle.Bold;
            titleTextComponent.alignment = TextAnchor.MiddleCenter;

            // スクロールビュー
            var scrollView = CreateScrollView(mainPanel, "ScrollView");
            var scrollRect = scrollView.GetComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0.1f);
            scrollRect.anchorMax = new Vector2(1, 0.8f);
            scrollRect.offsetMin = new Vector2(10, 50);
            scrollRect.offsetMax = new Vector2(-10, -10);

            // ボタンコンテナ
            var buttonContainer = scrollView.transform.Find("Viewport/Content").gameObject;

            foreach (var vehicle in vehicles)
            {
                CreateVehicleButton(buttonContainer, vehicle);
            }

            // キャンセルボタン
            var cancelButton = CreateButton(
                mainPanel,
                "CancelButton",
                LocalizationManager.GetLocalizedString("VehicleTeleporterDialog.Cancel")
            );
            var cancelRect = cancelButton.GetComponent<RectTransform>();
            cancelRect.anchorMin = new Vector2(0, 0);
            cancelRect.anchorMax = new Vector2(0.5f, 0.1f);
            cancelRect.offsetMin = new Vector2(10, 10);
            cancelRect.offsetMax = new Vector2(-5, 0);

            cancelButton
                .GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    CloseDialog();
                });
        }

        private static GameObject CreatePanel(GameObject parent, string name)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent.transform, false);

            var rectTransform = panel.AddComponent<RectTransform>();
            var image = panel.AddComponent<Image>();
            image.color = Color.white;

            return panel;
        }

        private static GameObject CreateText(GameObject parent, string name, string text)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent.transform, false);

            var rectTransform = textObj.AddComponent<RectTransform>();
            var textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.color = Color.white;
            textComponent.fontSize = 18;

            return textObj;
        }

        private static GameObject CreateScrollView(GameObject parent, string name)
        {
            var scrollView = new GameObject(name);
            scrollView.transform.SetParent(parent.transform, false);

            var rectTransform = scrollView.AddComponent<RectTransform>();
            var scrollRect = scrollView.AddComponent<ScrollRect>();
            var image = scrollView.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Viewport
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollView.transform, false);
            var viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            var viewportImage = viewport.AddComponent<Image>();
            viewportImage.color = Color.clear;
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            // Content
            var content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            var contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = new Vector2(0, 0);

            var contentImage = content.AddComponent<Image>();
            contentImage.color = Color.clear;

            // Vertical Layout Group
            var layoutGroup = content.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 5;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            // Content Size Fitter
            var contentSizeFitter = content.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;

            return scrollView;
        }

        private static void CreateVehicleButton(GameObject parent, VehicleInfo vehicle)
        {
            var button = CreateButton(parent, $"VehicleButton_{vehicle.TechType}", vehicle.Name);
            var buttonRect = button.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 50);

            var buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() =>
            {
                CloseDialog();
                TeleportVehicle(vehicle);
            });
        }

        private static GameObject CreateButton(GameObject parent, string name, string text)
        {
            var button = new GameObject(name);
            button.transform.SetParent(parent.transform, false);

            var rectTransform = button.AddComponent<RectTransform>();
            var image = button.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);

            var buttonComponent = button.AddComponent<Button>();
            buttonComponent.targetGraphic = image;

            // ボタンテキスト
            var textObj = CreateText(button, "Text", text);
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var textComponent = textObj.GetComponent<Text>();
            textComponent.fontSize = 16;
            textComponent.alignment = TextAnchor.MiddleCenter;

            return button;
        }

        private static void TeleportVehicle(VehicleInfo vehicle)
        {
            try
            {
                var player = Player.main;
                if (player == null)
                    return;

                ShowMessage(
                    LocalizationManager.GetLocalizedString(
                        "VehicleTeleporterMessages.Teleporting",
                        new Dictionary<string, string> { { "name", vehicle.Name } }
                    )
                );

                // 展開された乗り物の場合、直接テレポート
                TeleportDeployedVehicle(vehicle, player);
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Failed to teleport vehicle: {ex.Message}");
                ShowMessage(
                    LocalizationManager.GetLocalizedString(
                        "VehicleTeleporterMessages.TeleportFailed"
                    )
                );
            }
        }

        private static void TeleportDeployedVehicle(VehicleInfo vehicle, Player player)
        {
            if (vehicle.VehicleObject == null)
                return;

            var spawnPosition = GetSpawnPosition(player);
            var spawnRotation = GetSpawnRotation(player);

            // テレポート開始エフェクトを車両の現在位置で再生
            PlayTeleportEffect(vehicle.VehicleObject.transform.position, "start");
            PlayTeleportSound(vehicle.VehicleObject.transform.position, "start");

            // 乗り物を新しい位置に移動
            vehicle.VehicleObject.transform.position = spawnPosition;
            vehicle.VehicleObject.transform.rotation = spawnRotation;

            // 物理演算をリセット
            var rigidbody = vehicle.VehicleObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }

            // テレポート完了エフェクトを新しい位置で再生
            PlayTeleportEffect(spawnPosition, "end");
            PlayTeleportSound(spawnPosition, "end");

            ShowMessage(
                LocalizationManager.GetLocalizedString(
                    "VehicleTeleporterMessages.TeleportSuccess",
                    new Dictionary<string, string> { { "name", vehicle.Name } }
                )
            );
        }

        private static Vector3 GetSpawnPosition(Player player)
        {
            // Use the camera's forward direction for accurate player facing
            var cameraTransform = MainCamera.camera.transform;
            var forward = cameraTransform.forward;
            var up = cameraTransform.up;

            // プレイヤーの前方3メートル、上1メートルの位置（目の前）
            return player.transform.position + forward * 3f + up * 1f;
        }

        private static Quaternion GetSpawnRotation(Player player)
        {
            // Use the camera's rotation for accurate player facing
            return MainCamera.camera.transform.rotation;
        }

        private static System.Collections.IEnumerator SpawnVehicleAsync(
            TechType techType,
            Vector3 position,
            Quaternion rotation,
            string vehicleName
        )
        {
            var task = CraftData.GetPrefabForTechTypeAsync(techType, false);
            yield return task;

            var prefab = task.GetResult();
            if (prefab != null)
            {
                var spawnedVehicle = UWE.Utils.InstantiateDeactivated(prefab, position, rotation);
                if (spawnedVehicle != null)
                {
                    spawnedVehicle.SetActive(true);
                    ShowMessage(
                        LocalizationManager.GetLocalizedString(
                            "VehicleTeleporterMessages.TeleportSuccess",
                            new Dictionary<string, string> { { "name", vehicleName } }
                        )
                    );
                }
            }
            else
            {
                ShowMessage(
                    LocalizationManager.GetLocalizedString(
                        "VehicleTeleporterMessages.TeleportFailed"
                    )
                );
            }
        }

        public static bool IsDialogOpen()
        {
            return _currentDialog != null;
        }

        public static void CloseDialog()
        {
            if (_currentDialog != null)
            {
                Destroy(_currentDialog);
                _currentDialog = null;
            }
        }

        private static void ShowMessage(string message)
        {
            ErrorMessage.AddMessage(message);
        }

        // テレポートエフェクトを再生（簡易版）
        private static void PlayTeleportEffect(Vector3 position, string phase)
        {
            try
            {
                // 簡易エフェクトのみ使用
                if (phase == "start")
                {
                    CreateSimpleParticleEffect(position, new Color(0.5f, 0.2f, 1.0f, 1.0f));
                }
                else
                {
                    CreateSimpleParticleEffect(position, new Color(0.2f, 1.0f, 0.5f, 1.0f));
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogWarning($"Failed to play teleport effect: {ex.Message}");
            }
        }

        // Warper SFX をWarperプレハブから抽出してキャッシュする
        private static System.Collections.IEnumerator InitWarperSfxAsync()
        {
            try
            {
                var task = CraftData.GetPrefabForTechTypeAsync(TechType.Warper, false);
                yield return task;
                var warperPrefab = task.GetResult();
                if (warperPrefab == null)
                {
                    Plugin.Log.LogWarning("Failed to get Warper prefab for SFX extraction");
                    _warperSfxLoading = false;
                    yield break;
                }

                // サウンドイベントの抽出（FMOD系コンポーネントを反射で総当たり）
                var allComps = warperPrefab.GetComponentsInChildren<Component>(true);
                foreach (var comp in allComps)
                {
                    if (comp == null)
                        continue;
                    var type = comp.GetType();
                    var tname = type.Name.ToLowerInvariant();
                    if (!tname.Contains("fmod"))
                        continue;

                    string path = null;
                    // よくあるプロパティ/フィールド名を順に試す
                    var candidates = new string[] { "eventPath", "path", "Event", "eventName" };
                    foreach (var member in candidates)
                    {
                        var prop = type.GetProperty(
                            member,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        );
                        if (prop != null && prop.PropertyType == typeof(string))
                        {
                            path = prop.GetValue(comp) as string;
                            if (!string.IsNullOrEmpty(path))
                                break;
                        }
                        var field = type.GetField(
                            member,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        );
                        if (field != null && field.FieldType == typeof(string))
                        {
                            path = field.GetValue(comp) as string;
                            if (!string.IsNullOrEmpty(path))
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(path))
                        continue;
                    var lname = comp.gameObject.name.ToLowerInvariant();
                    if (
                        lname.Contains("out")
                        || lname.Contains("open")
                        || lname.Contains("start")
                        || path.Contains("warp_out")
                        || path.Contains("teleport_out")
                    )
                    {
                        _warperOutEventPaths.Add(path);
                    }
                    else if (
                        lname.Contains("in")
                        || lname.Contains("close")
                        || lname.Contains("end")
                        || path.Contains("warp_in")
                        || path.Contains("teleport_in")
                    )
                    {
                        _warperInEventPaths.Add(path);
                    }
                    else
                    {
                        _warperOutEventPaths.Add(path);
                        _warperInEventPaths.Add(path);
                    }
                }

                _warperSfxInitialized = true;
            }
            finally
            {
                _warperSfxLoading = false;
            }
        }

        // シンプルなパーティクルエフェクトを作成（フォールバック用）
        private static void CreateSimpleParticleEffect(Vector3 position, Color color)
        {
            try
            {
                var effectObject = new GameObject("TeleportEffect");
                effectObject.transform.position = position;

                var particleSystem = effectObject.AddComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.startLifetime = 2f;
                main.startSpeed = 5f;
                main.startSize = 0.5f;
                main.startColor = color;
                main.maxParticles = 100;

                var emission = particleSystem.emission;
                emission.rateOverTime = 50f;

                var shape = particleSystem.shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 1f;

                particleSystem.Play();

                // エフェクトを3秒後に削除
                UnityEngine.Object.Destroy(effectObject, 3f);
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogWarning($"Failed to create simple particle effect: {ex.Message}");
            }
        }

        // テレポートサウンドを再生（可能ならWarperの本物を優先）
        private static void PlayTeleportSound(Vector3 position, string phase)
        {
            try
            {
                if (phase == "start")
                {
                    // まず抽出済みのWarperイベントを優先
                    if (_warperSfxInitialized && _warperOutEventPaths.Count > 0)
                    {
                        foreach (var p in _warperOutEventPaths)
                        {
                            try
                            {
                                RuntimeManager.PlayOneShot(p, position);
                                break;
                            }
                            catch { }
                        }
                        return;
                    }

                    // 既知のイベント一覧（フォールバック）
                    var warperSoundEvents = new string[]
                    {
                        "event:/creature/warper/warp_out",
                        "event:/creature/warper/teleport_out",
                        "event:/creature/warper/portal_open",
                        "event:/tools/scanner/new_encyclopediea",
                        "event:/interface/option_select",
                    };
                    foreach (var soundEvent in warperSoundEvents)
                    {
                        try
                        {
                            RuntimeManager.PlayOneShot(soundEvent, position);
                            break;
                        }
                        catch { }
                    }
                }
                else if (phase == "end")
                {
                    if (_warperSfxInitialized && _warperInEventPaths.Count > 0)
                    {
                        foreach (var p in _warperInEventPaths)
                        {
                            try
                            {
                                RuntimeManager.PlayOneShot(p, position);
                                break;
                            }
                            catch { }
                        }
                        return;
                    }

                    var warperSoundEvents = new string[]
                    {
                        "event:/creature/warper/warp_in",
                        "event:/creature/warper/teleport_in",
                        "event:/creature/warper/portal_close",
                        "event:/sub/cyclops/sonar_ping",
                        "event:/interface/option_select",
                    };
                    foreach (var soundEvent in warperSoundEvents)
                    {
                        try
                        {
                            RuntimeManager.PlayOneShot(soundEvent, position);
                            break;
                        }
                        catch { }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Plugin.Log.LogError($"Failed to play warper teleport sound: {ex.Message}");
            }
        }
    }

    public class VehicleInfo
    {
        public TechType TechType { get; set; }
        public string Name { get; set; }
        public bool IsStored { get; set; }
        public Pickupable StoredItem { get; set; }
        public GameObject VehicleObject { get; set; }
    }
}
