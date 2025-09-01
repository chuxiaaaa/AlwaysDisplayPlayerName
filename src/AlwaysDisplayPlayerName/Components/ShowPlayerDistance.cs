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

        // 固定距离文本尺寸常量
        private const float FIXED_DISTANCE_WIDTH = 200f;
        private const float FIXED_DISTANCE_HEIGHT = 50f;

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
        /// 是否已经设置过位置
        /// </summary>
        private bool _hasSetPosition = false;

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
                SetVisibility(false);
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

            // 设置固定尺寸
            _distanceRect.sizeDelta = new Vector2(FIXED_DISTANCE_WIDTH * 2f, FIXED_DISTANCE_HEIGHT);

            Plugin.Log.LogInfo($"ShowPlayerDistance initialized for character: {targetPlayerName?.characterInteractable?.character?.name ?? "Unknown"} with child: {targetPlayerName?.name ?? "Unknown"}");

            _lastCharacter = _playerName.characterInteractable.character;

            // 初始化距离显示状态
            _shouldShowDistance = ShouldShowDistance();

            // 如果距离显示状态为false，则不显示距离
            if (!_shouldShowDistance)
            {
                SetVisibility(false);
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
                    SetVisibility(false);
                    return;
                }

                // 更新距离
                UpdateDistance();
            }
            catch (Exception e)
            {
                Plugin.Log.LogError($"Error in OnUpdate: {e.ToString()}");

                SetVisibility(false);
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
                if (_shouldShowDistance && _playerName.text != null && !_hasSetPosition)
                {
                    var playerNamePos = _playerNameRect.anchoredPosition;

                    // 设置X和Y坐标位置
                    var initialY = playerNamePos.y + FIXED_DISTANCE_HEIGHT + FIXED_DISTANCE_HEIGHT * 0.5f + 5f;
                    var playerNameCenterX = playerNamePos.x + _playerNameRect.sizeDelta.x * (0.5f - _playerNameRect.pivot.x);
                    var targetX = playerNameCenterX - FIXED_DISTANCE_WIDTH * (0.5f - _distanceRect.pivot.x);

                    _distanceRect.anchoredPosition = new Vector2(targetX, initialY);
                    _hasSetPosition = true;
                    Plugin.Log.LogInfo($"Sync UI for character: {_playerName.characterInteractable.character.name}");
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
        private void SetVisibility(bool visible, bool setParentVisible = false)
        {
            // 如果画布组不为空，则设置画布组透明度
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? 1f : 0f;
                if (setParentVisible)
                {
                    if (_playerName.text != null)
                        _playerName.text.enabled = visible;
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
                    SetVisibility(false, true);
                }
                else
                {
                    SetVisibility(true, true);
                }
            }
            else
            {
                // 设置距离可见性
                SetVisibility(true);
            }
            // 计算距离
            var distance = Vector3.Distance(Character.observedCharacter.Center, _playerName.characterInteractable.character.Center);
            distanceText.text = $"{distance:F1}m";
        }
    }
}
