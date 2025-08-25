using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace AlwaysDisplayPlayerName.Compatibility
{
    internal class PeakCinema_Compat
    {
        internal static bool Loaded { get { return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.github.megalon.peakcinema"); } }

        internal static CinemaCamera? Cinema
        {
            get
            {
                if (_cinemaCamera == null && Time.time - lastFindTime > 1f)
                {
                    lastFindTime = Time.time;
                   _cinemaCamera = UnityEngine.Object.FindAnyObjectByType<CinemaCamera>();
                }
                return _cinemaCamera;
            }
        }

        private static float lastFindTime;

        private static CinemaCamera _cinemaCamera = null!;

    }
}
