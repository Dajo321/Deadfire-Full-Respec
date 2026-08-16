using BepInEx;
using DeadfireFullRespec;
using Game;
using Game.GameData;
using Game.UI;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using static Game.UI.UICharacterCreationManager;
using static Game.WindZoneReacter;

[HarmonyPatch(typeof(Game.UI.UIStoreRespecRow), "OnAcceptRespec")]
public static class Patch_OnAcceptRespec
{
    static bool Prefix(Game.UI.UIStoreRespecRow __instance, object sender)
    {
        FullRespecPlugin.Log?.LogInfo("Intercepted OnAcceptRespec.");

        var selected = __instance.SelectedObject;
        if (selected == null)
        {
            FullRespecPlugin.Log?.LogError("SelectedObject is null!");
            return true;
        }

        FullRespecState.IsFullRespec = true;
        FullRespecState.TargetCharacterStats = selected;

        FullRespecPlugin.Log?.LogInfo("Launching FULL character creation via NewPlayer.");

        var mgr = UICharacterCreationManager.Instance;
        if (mgr == null)
        {
            FullRespecPlugin.Log?.LogError("[FullRespec] UICharacterCreationManager.Instance is null!");
            return true;
        }

        try
        {
            mgr.OpenCharacterCreation(
                UICharacterCreationManager.CharacterCreationType.NewPlayer,
                selected.gameObject,
                0,              // starting level index
                1,              // level 1
                selected.Experience,
                false           // isRespec = false
            );
        }
        catch (Exception ex)
        {
            FullRespecPlugin.Log?.LogError("[FullRespec] Failed to open full character creation: " + ex);
            return true;
        }

        return false; // skip original respec logic
    }

    static void PostFix(Game.UI.UIStoreRespecRow __instance)
    {
        FullRespecState.IsFullRespec = false;
    }
}


[HarmonyPatch(typeof(UICharacterCreationManager), "Show")]
public static class Patch_Show
{
    private static bool Prefix(UICharacterCreationManager __instance)
    {
        if (FullRespecState.IsFullRespec == false)
        {
            return true;
        }

        FullRespecPlugin.Log?.LogError("Hooking UICharacterCreationManager.Show()");

        // SetMainCameraActive(false);
        __instance.SetMainCameraActive(false);

        // GameRender.UpdateHairTransparencyLOD(false);
        GameRender.UpdateHairTransparencyLOD(IsGameView: false);

        // Disable everything that starts disabled.
        /*
        foreach (GameObject gameObject in __instance.StartsDisabled)
        {
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }
        */
        // if (SelectedObject == null)
        //     SelectedObject = new CharacterCreationCharacter();
        if (__instance.SelectedObject == null)
        {
            __instance.SelectedObject = new CharacterCreationCharacter();
        }

        // Configure the paperdoll camera according to creation type.
        switch (__instance.CreationType)
        {
            case UICharacterCreationManager.CharacterCreationType.NewPlayer:
                Paperdoll.CreateCameraCharacterCreation();
                break;

            case UICharacterCreationManager.CharacterCreationType.NewCompanion:
                Paperdoll.CreateCameraRecruitment();
                break;

            case UICharacterCreationManager.CharacterCreationType.LevelUp:
                Paperdoll.CreateCameraLevelUp();
                break;
        }

        // Paperdoll.SetRenderSize(...)
        Paperdoll.SetRenderSize(
            new Rect(
                0f,
                0f,
                __instance.CharacterRenderZone.transform.localScale.x,
                __instance.CharacterRenderZone.transform.localScale.y));

        __instance.CharacterRenderZone.mainTexture = Paperdoll.RenderImage;

        // Disable all root controllers.
        for (int i = 0; i < __instance.RootControllers.Length; i++)
        {
            __instance.RootControllers[i].gameObject.SetActive(false);
        }

        // m_rootController = RootControllers[(int)CreationType];
        // m_rootController is protected, so access it through Traverse.
        UICharacterCreationController rootController =
            __instance.RootControllers[(int)__instance.CreationType];

        Traverse.Create(__instance)
            .Field("m_rootController")
            .SetValue(rootController);

        rootController.gameObject.SetActive(true);

        __instance.CalculateAllControllers();
        /*
        // New player / new companion appearance setup.
        if (__instance.CreationType == UICharacterCreationManager.CharacterCreationType.NewPlayer || 
                __instance.CreationType == UICharacterCreationManager.CharacterCreationType.NewCompanion)
        {
            GenericAppearance genericAppearance =
                __instance.TargetCharacter.GetComponent<GenericAppearance>();

            if ((bool)genericAppearance)
            {
                genericAppearance.DestroyAppearance();
                ResourceManager.DestroyComponent(genericAppearance);
            }

            NPCAppearance npcAppearance =
                __instance.TargetCharacter.GetComponent<NPCAppearance>();

            if (!npcAppearance)
            {
                ResourceManager.AddComponent<NPCAppearance>(
                    __instance.TargetCharacter);
            }
        }
        */

        // ActiveCharacter.GetFrom(TargetCharacter);
        __instance.ActiveCharacter.GetFrom(__instance.TargetCharacter);

        if (__instance.CreationType ==
            UICharacterCreationManager.CharacterCreationType.LevelUp)
        {
            __instance.LevelUpNotification.gameObject.SetActive(true);
            __instance.BeginLevelUp();
        }
        else
        {
            __instance.LevelUpNotification.gameObject.SetActive(false);
            __instance.BeginNewCharacter();
        }

        // m_onSelectedObjectChanged.Trigger();
        // This field is protected, so retrieve it through Traverse.
        Traverse.Create(__instance)
            .Field("m_onSelectedObjectChanged")
            .Method("Trigger")
            .GetValue();

        // Paperdoll.LoadOrCreatePaperDoll(TargetCharacter);
        Paperdoll.LoadOrCreatePaperDoll(__instance.TargetCharacter);

        rootController.Activate();
        rootController.Show();

        if (__instance.CreationType ==
            UICharacterCreationManager.CharacterCreationType.LevelUp)
        {
            __instance.SetLastPickedSubraceForRace(
                __instance.ActiveCharacter.Race,
                __instance.ActiveCharacter.Subrace);
        }
        /*
        else
        {
            // ActiveCharacter.Gender =
            //     random Female/Male with a 50/50 split.
            __instance.ActiveCharacter.Gender =
                !(OEIRandom.FloatValue() < 0.5f)
                    ? CustomGameDataLookup.GetGenderDataObject(Gender.Female)
                    : CustomGameDataLookup.GetGenderDataObject(Gender.Male);

            // Populate LastPickedSubrace for every valid character-creation race.
            for (int i = 0;
                 i < CharacterProgressionGameData.Instance
                     .CharacterCreationValidRaces.Count;
                 i++)
            {
                RaceGameData raceGameData =
                    CharacterProgressionGameData.Instance
                        .CharacterCreationValidRaces[i];

                if (!__instance.LastPickedSubrace.ContainsKey(raceGameData))
                {
                    __instance.LastPickedSubrace.Add(
                        raceGameData,
                        OEIRandom.Element(
                            raceGameData.CharacterCreationSubraces));
                }
            }

            __instance.InitializeRace(
                CustomGameDataLookup.GetRace(Race.Human));
        }
        */
        // CharacterCreationBackground.SetPath(Backgrounds[(int)CreationType]);
        __instance.CharacterCreationBackground.SetPath(
            __instance.Backgrounds[(int)__instance.CreationType]);

        // Clear();
        __instance.Clear();

        // Skip the original Show().
        return false;
    }
}