using BepInEx;
using HarmonyLib;
using MoreOrLessStars.Helpers;
using System;
using System.Reflection;
using UnityEngine.UI;

namespace MoreOrLessStars {
    [BepInPlugin("org.bepinex.plugins.moreorlessstars", "More or less stars", "1.0.0.0")]
    [BepInProcess("DSPGAME.exe")]
    public class MoreOrLessStars : BaseUnityPlugin {
        // Apply all patches
        void Start() {
            Harmony.CreateAndPatchAll(typeof(MoreOrLessStars));
        }

        private static readonly int desiredMinStars = 10;
        private static readonly int desiredMaxStars = 200;

        // Change the minimum and maximum values of the galaxy select slider
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "UpdateUIDisplay")]
        public static void Patch(ref UIGalaxySelect __instance, GalaxyData galaxy) {
            Type type = __instance.GetType();
            FieldInfo fi = type.GetField("starCountSlider", BindingFlags.NonPublic | BindingFlags.Instance);
            Slider slider = (Slider)fi.GetValue(__instance);
            slider.minValue = desiredMinStars;
            slider.maxValue = desiredMaxStars;
        }

        // Remove the hard-coded star limits
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static bool Patch(ref UIGalaxySelect __instance, ref float val) {
            // Gather required private fields
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            Slider starCountSlider = ReflectionHelper.GetField<Slider>(__instance, "starCountSlider", flags);
            GameDesc gameDesc = ReflectionHelper.GetField<GameDesc>(__instance, "gameDesc", flags);

            // Replicate the code of the original method
            int num = (int)( starCountSlider.value + 0.1f );
            if ( num < desiredMinStars ) {
                num = desiredMinStars;
            }
            else if ( num > desiredMaxStars ) {
                num = desiredMaxStars;
            }
            if ( num != gameDesc.starCount ) {
                gameDesc.starCount = num;
                __instance.SetStarmapGalaxy();
            }

            // Return to prevent the call of the original method
            return false;
        }
    }
}
