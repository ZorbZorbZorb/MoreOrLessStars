using HarmonyLib;
using System.Security;
using System.Security.Permissions;
using BepInEx;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MoreOrLessStars {
    [BepInPlugin("org.bepinex.plugins.moreorlessstars", "More or less stars", "1.0.1")]
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
            if (__instance.starCountSlider.minValue != desiredMinStars) {
                __instance.starCountSlider.minValue = desiredMinStars;
            }
            if (__instance.starCountSlider.maxValue != desiredMaxStars) {
                __instance.starCountSlider.maxValue = desiredMaxStars;
            }
        }

        // Remove the hard-coded star limits
        [HarmonyPrefix, HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static bool Patch(ref UIGalaxySelect __instance, ref float val) {

            // Replicate the code of the original method
            int num = (int)( __instance.starCountSlider.value + 0.1f );
            if ( num < desiredMinStars ) {
                num = desiredMinStars;
            }
            else if ( num > desiredMaxStars ) {
                num = desiredMaxStars;
            }
            if ( num != __instance.gameDesc.starCount ) {
                __instance.gameDesc.starCount = num;
                __instance.SetStarmapGalaxy();
            }

            // Return to prevent the call of the original method
            return false;
        }
    }
}
