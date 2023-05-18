
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace MC_SVSpawnHunters
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginGuid = "mc.starvalor.spawnhunters";
        public const string pluginName = "SV Spawn Hunters";
        public const string pluginVersion = "1.0.1";

        public ConfigEntry<KeyCodeSubset> modifierKey;
        public ConfigEntry<KeyCodeSubset> mainKey;

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Main));
            modifierKey = Config.Bind("Config",
                "Modifier key",
                KeyCodeSubset.RightControl,
                "Set to None to only use main key.  Otherwise modifier key + main key will spawn.");
            mainKey = Config.Bind("Config",
                "Main key",
                KeyCodeSubset.Q,
                "");
        }

        public void Update()
        {
            if (((modifierKey.Value != KeyCodeSubset.None && Input.GetKey((KeyCode) modifierKey.Value)) ||
                modifierKey.Value == KeyCodeSubset.None) &&
                Input.GetKeyDown((KeyCode) mainKey.Value))
                {
                    int faction = GameData.data.GetCurrentSector().factionControl;
                    if (faction < 0 || faction > AccessTools.StaticFieldRefAccess<int>(typeof(FactionDB), "factionsCount") - 1)
                        faction = 0;

                    MethodInfo CreateEnemyRoutine = AccessTools.Method(typeof(GameManager), "CreateEnemyRoutine");
                    GameManager.instance.StartCoroutine(
                        (System.Collections.IEnumerator)CreateEnemyRoutine.Invoke(GameManager.instance, new object[] { 0f,  faction })
                        );
                }
        }
    }
}
