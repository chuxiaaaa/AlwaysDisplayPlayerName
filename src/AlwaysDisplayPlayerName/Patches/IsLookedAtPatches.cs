using System.Collections;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AlwaysDisplayPlayerName.Components;

namespace AlwaysDisplayPlayerName.Patches
{
    [HarmonyPatch(typeof(IsLookedAt))]
    [HarmonyWrapSafe]
    public static class IsLookedAtPatches
    {
        /// <summary>
        /// 补丁IsLookedAt.Update方法，更新PlayerName的可见性
        /// </summary>
        /// <param name="__instance">IsLookedAt实例</param>
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        public static bool IsLookAtUpdatePath(IsLookedAt __instance)
        {
            if (!Plugin.configEnable.Value)
            {
                return true;
            }

            var visible = false;
            var angle = Vector3.Angle(MainCamera.instance.transform.forward, __instance.transform.position - MainCamera.instance.transform.position);

            if (angle < Plugin.configVisibleAngle.Value)
            {
                visible = true;
            }

            if (__instance.mouth.character.data.isBlind)
            {
                visible = Plugin.configDisplayWhenBlind.Value;
            }

            var indexField = AccessTools.Field(typeof(IsLookedAt), "index");
            var index = (int)indexField.GetValue(__instance);
            GUIManager.instance.playerNames.UpdateName(index, __instance.playerNamePos.position, visible, __instance.mouth.amplitudeIndex);
            return false;
        }

        /// <summary>
        /// 补丁IsLookedAt.Start方法，在PlayerName对象初始化时创建距离显示
        /// </summary>
        /// <param name="__instance">IsLookedAt实例</param>
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void IsLookedAtStartPatch(IsLookedAt __instance)
        {
            if (__instance == null || GUIManager.instance?.playerNames == null || __instance.characterInteractible.character == Character.localCharacter)
            {
                return;
            }

            var indexField = AccessTools.Field(typeof(IsLookedAt), "index");
            if (indexField != null)
            {
                var index = (int)indexField.GetValue(__instance);
                Plugin.Log.LogInfo($"IsLookedAt Start - Index: {index}");

                // 通过GUIManager获取对应的PlayerName
                var playerName = GetPlayerNameByIndex(index);
                var showDistance = playerName.GetComponent<ShowPlayerDistance>();
                if (playerName != null && showDistance == null)
                {
                    Plugin.Log.LogInfo($"Creating distance display for PlayerName at index {index}: {playerName.name}");
                    Plugin.Instance.StartCoroutine(DelayedCreateDistanceDisplay(playerName));
                }
            }
        }

        /// <summary>
        /// 通过索引获取PlayerName对象
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>PlayerName对象</returns>
        private static PlayerName GetPlayerNameByIndex(int index)
        {
            // 通过GUIManager.instance.playerNames获取PlayerName对象
            if (GUIManager.instance?.playerNames?.playerNameText != null &&
                index >= 0 && index < GUIManager.instance.playerNames.playerNameText.Length)
            {
                return GUIManager.instance.playerNames.playerNameText[index];
            }
            return null!;
        }

        /// <summary>
        /// 延迟创建距离显示
        /// </summary>
        /// <param name="originalPlayerName">原始PlayerName对象</param>
        /// <returns>协程</returns>
        public static IEnumerator DelayedCreateDistanceDisplay(PlayerName originalPlayerName)
        {
            yield return null;
            CreateDistanceDisplay(originalPlayerName);
        }

        /// <summary>
        /// 创建距离显示
        /// </summary>
        /// <param name="originalPlayerName">原始PlayerName对象</param>
        public static void CreateDistanceDisplay(PlayerName originalPlayerName)
        {
            var originalTextRect = originalPlayerName.text.rectTransform;
            var distanceObj = new GameObject("DistanceDisplay");
            var distanceRect = distanceObj.AddComponent<RectTransform>();
            var layoutElement = distanceObj.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;

            // 设置父对象
            distanceRect.SetParent(originalTextRect.parent);

            // 添加TMP_Text组件
            var distanceText = distanceObj.AddComponent<TextMeshProUGUI>();

            // 设置文本属性
            distanceText.text = "0.0m";
            distanceText.alignment = TextAlignmentOptions.Center;
            distanceText.font = originalPlayerName.text.font;
            distanceText.fontSize = originalPlayerName.text.fontSize * 1.4f;
            distanceText.color = Color.white;

            // 使用相同材质
            if (originalPlayerName.text.fontSharedMaterial != null)
                distanceText.fontSharedMaterial = originalPlayerName.text.fontSharedMaterial;

            // 添加 ShowPlayerDistance 组件
            var showDistance = distanceObj.AddComponent<ShowPlayerDistance>();
            showDistance.Initialize(distanceText, originalPlayerName);

            Plugin.Log.LogInfo($"Created distance display for player name: {originalPlayerName.name}");
        }
    }
}

