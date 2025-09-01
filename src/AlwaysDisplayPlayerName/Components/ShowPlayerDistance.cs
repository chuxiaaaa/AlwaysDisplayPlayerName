using UnityEngine;
using TMPro;
using HarmonyLib;
using System;
using AlwaysDisplayPlayerName.Common;
using AlwaysDisplayPlayerName.Compatibility;

namespace AlwaysDisplayPlayerName.Components
{
    /// <summary>
    /// 显示玩家距离
    /// </summary>
    public class ShowPlayerDistance : MonoBehaviour
    {
        private const float UPDATE_INTERVAL = 0.1f;
        private const float DISTANCE_OFFSET_Y = 5f;
        private const float HEIGHT_MULTIPLIER = 0.5f;

        /// <summary>
        /// 距离文本
        /// </summary>
        public TextMeshProUGUI distanceText = null!;

        /// <summary>
        /// 玩家名称组件
        /// </summary>
        private PlayerName _playerName = null!;

        /// <summary>
        /// 画布组
        /// </summary>
        private CanvasGroup _canvasGroup = null!;

        /// <summary>
        /// 是否显示距离
        /// </summary>
        private bool _shouldShowDistance = false;

        /// <summary>
        /// 上一次角色
        /// </summary>
        private Character _lastCharacter = null!;

        /// <summary>
        /// 玩家名称RectTransform
        /// </summary>
        private RectTransform _playerNameRect = null!;

        /// <summary>
        /// 距离RectTransform
        /// </summary>
        private RectTransform _distanceRect = null!;

        /// <summary>
        /// 主更新计时器
        /// </summary>
        private IntervalTimer _mainUpdateTimer = null!;

        /// <summary>
        /// UI更新计时器
        /// </summary>
        private IntervalTimer _uiUpdateTimer = null!;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Awake()
        {
            if (Plugin.configShowDistance != null)
                Plugin.configShowDistance.SettingChanged += OnConfigChanged;

            _mainUpdateTimer = new IntervalTimer(UPDATE_INTERVAL, OnMainUpdate);
            _uiUpdateTimer = new IntervalTimer(UPDATE_INTERVAL * 0.5f, OnUIUpdate);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        private void OnDestroy()
        {
            if (Plugin.configShowDistance != null)
            {
                Plugin.configShowDistance.SettingChanged -= OnConfigChanged;
            }
        }

        /// <summary>
        /// 配置改变
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">事件参数</param>
        private void OnConfigChanged(object sender, EventArgs e)
        {
            // 更新显示状态标志
            _shouldShowDistance = ShouldShowDistance();

            // 根据新的状态控制显示/隐藏
            if (_shouldShowDistance)
            {
                UpdateDistance();
            }
            else
            {
                SetDistanceVisibility(false);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="text">距离文本</param>
        /// <param name="targetPlayerName">目标玩家名称</param>
        public void Initialize(TextMeshProUGUI text, PlayerName targetPlayerName)
        {
            distanceText = text;
            _playerName = targetPlayerName;

            _canvasGroup = distanceText.gameObject.AddComponent<CanvasGroup>();

            // 缓存RectTransform引用
            _playerNameRect = targetPlayerName.text.rectTransform;
            _distanceRect = distanceText.rectTransform;

            // 设置距离RectTransform的锚点、pivot和位置
            _distanceRect.anchorMin = _playerNameRect.anchorMin;
            _distanceRect.anchorMax = _playerNameRect.anchorMax;
            _distanceRect.pivot = _playerNameRect.pivot;
            _distanceRect.anchoredPosition = _playerNameRect.anchoredPosition;

            Plugin.Log.LogInfo($"ShowPlayerDistance initialized for character: {targetPlayerName?.characterInteractable?.character?.name ?? "Unknown"} with child: {targetPlayerName?.name ?? "Unknown"}");

            _lastCharacter = _playerName.characterInteractable.character;

            // 初始化距离显示状态
            _shouldShowDistance = ShouldShowDistance();

            // 如果距离显示状态为false，则不显示距离
            if (!_shouldShowDistance)
            {
                SetDistanceVisibility(false);
            }
        }

        private void Update()
        {
            _mainUpdateTimer.Update();
            _uiUpdateTimer.Update();
        }

        /// <summary>
        /// 主更新
        /// </summary>
        private void OnMainUpdate()
        {
            try
            {
                // 如果角色发生变化，则更新角色
                var current = _playerName?.characterInteractable?.character;
                if (current == null || current != _lastCharacter)
                {
                    Plugin.Log.LogInfo($"Character changed, updating state");
                    _lastCharacter = current!;
                }

                // 如果配置显示距离为false，则不显示距离
                _shouldShowDistance = ShouldShowDistance();

                // 如果配置显示距离为false，则不显示距离
                if (!_shouldShowDistance)
                {
                    SetDistanceVisibility(false);
                    return;
                }

                // 更新距离
                UpdateDistance();
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error in OnUpdate: {e.ToString()}");

                SetDistanceVisibility(false);
                _shouldShowDistance = false;
            }
        }

        /// <summary>
        /// UI更新
        /// </summary>
        private void OnUIUpdate()
        {
            try
            {
                if (_shouldShowDistance && _playerName?.text != null)
                {
                    // 使用缓存的RectTransform引用
                    var playerNameSize = _playerNameRect.sizeDelta;
                    var playerNamePos = _playerNameRect.anchoredPosition;
                    var playerNameHeight = playerNameSize.y;
                    var distanceHeight = _distanceRect.sizeDelta.y;

                    // 持续同步Y坐标位置
                    var targetY = playerNamePos.y + playerNameHeight + distanceHeight * HEIGHT_MULTIPLIER + DISTANCE_OFFSET_Y;

                    // 如果父对象不同，先设置好宽度和X坐标
                    if (_distanceRect.parent != _playerNameRect)
                    {
                        // 先设置尺寸和X坐标
                        _distanceRect.sizeDelta = new Vector2(playerNameSize.x * 3f, playerNameSize.y);

                        // 考虑锚点计算居中的X位置
                        var playerNameCenterX = playerNamePos.x + playerNameSize.x * (0.5f - _playerNameRect.pivot.x);
                        var targetX = playerNameCenterX - _distanceRect.sizeDelta.x * (0.5f - _distanceRect.pivot.x);

                        // 先设置X坐标，Y坐标等设置父对象后再同步
                        _distanceRect.anchoredPosition = new Vector2(targetX, targetY);

                        // 然后设置父对象
                        _distanceRect.SetParent(_playerNameRect, true);
                        Plugin.Log.LogInfo($"Sync UI for character: {_playerName.characterInteractable.character.name}");
                    }

                    // 只更新Y坐标，保持X坐标不变，为什么要一直更新是因为偶尔会看到位置比较高，不在理想位置，所以需要一直更新（偷懒）
                    var currentPos = _distanceRect.anchoredPosition;
                    _distanceRect.anchoredPosition = new Vector2(currentPos.x, targetY);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error in OnUISync: {e.ToString()}");
            }
        }

        /// <summary>
        /// 设置距离可见性
        /// </summary>
        /// <param name="visible">可见性</param>
        private void SetDistanceVisibility(bool visible, bool setParent = false)
        {
            // 如果画布组不为空，则设置画布组透明度
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
                if (setParent)
                {
                    var distanceDisplay = _canvasGroup.gameObject;
                    var text = distanceDisplay.transform.parent.gameObject;
                    var textMeshProUGUI = text.GetComponentInChildren<TextMeshProUGUI>();
                    if (textMeshProUGUI != null)
                    {
                        textMeshProUGUI.enabled = visible;
                    }
                }
            }

        }

        /// <summary>
        /// 是否显示距离
        /// </summary>
        /// <returns>是否显示距离</returns>
        private bool ShouldShowDistance()
        {
            // 如果配置显示距离为false，则不显示距离
            if (!Plugin.configShowDistance.Value)
            {
                Plugin.Log.LogDebug($"ShowDistance is disabled");
                return false;
            }

            // 如果玩家名称组件为空，则不显示距离
            if (_playerName.characterInteractable == null)
            {
                Plugin.Log.LogDebug($"Character interactable is null");
                return false;
            }

            // 如果玩家名称组件的角色的观察者是当前角色，则不显示距离
            if (_playerName.characterInteractable.character == Character.observedCharacter)
            {
                Plugin.Log.LogDebug($"Player character is observed character");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新距离
        /// </summary>
        private void UpdateDistance()
        {
            if (PeakCinema_Compat.Loaded)
            {
                if (PeakCinema_Compat.Cinema != null && PeakCinema_Compat.Cinema.on)
                {
                    SetDistanceVisibility(false, true);
                }
                else
                {
                    SetDistanceVisibility(true, true);
                }
            }
            else
            {
                // 设置距离可见性
                SetDistanceVisibility(true);
            }
            // 计算距离
            var distance = Vector3.Distance(Character.observedCharacter.Center, _playerName.characterInteractable.character.Center);
            distanceText.text = $"{distance:F1}m";
        }
    }
}
