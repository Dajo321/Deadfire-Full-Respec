using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace DeadfireFullRespec
{
    [BepInPlugin("com.daniel.deadfire.fullrespec", "Deadfire Full Respec", "0.1.0")]
    [BepInProcess("PillarsOfEternityII.exe")]
    public class FullRespecPlugin : BaseUnityPlugin
    {
        private Harmony _harmony;
        public static ManualLogSource Log { get; private set; }

        private void Awake()
        {
            Log = Logger;
            Log.LogMessage("Plugin started.");
            _harmony = new Harmony("com.daniel.deadfire.fullrespec");
            try
            {
                _harmony.PatchAll();
                Log.LogMessage("Harmony patches applied.");
            }
            catch (System.Exception ex)
            {
                Log.LogMessage("PatchAll() failed: " + ex);
            }
        }

        private void OnDestroy()
        {
            try { _harmony?.UnpatchSelf(); }
            catch (System.Exception ex)
            {
                Log.LogMessage("UnpatchSelf() failed: " + ex);
            }
        }
    }
}
