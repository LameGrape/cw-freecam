﻿using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;
using System.Security;

namespace freecam;

[ContentWarningPlugin(Plugin.PLUGIN_GUID, Plugin.PLUGIN_VERSION, true)]
[BepInPlugin(Plugin.PLUGIN_GUID, "freecam", Plugin.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "raisin.plugin.freecam";
    public const string PLUGIN_VERSION = "1.1.2";

    public static bool isFreeCam = false;
    public static bool isHud = true;

    private void Awake()
    {
        Logger.LogInfo("loaded freecam");

        SceneManager.activeSceneChanged += ApplyFreecam;

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    private void ApplyFreecam(Scene current, Scene next)
    {
        if (next.name == "NewMainMenu") return; // no freecam on main menu
        isFreeCam = false;
        Camera.main.gameObject.AddComponent<FreeCam>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerController), "Movement")]
    public static bool PlayerMovementPatch()
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerController), "SimpleMovement")]
    public static bool PlayerSimpleMovementPatch()
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerController), "TryJump")]
    public static bool PlayerJumpPatch()
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MainCameraMovement), "LateUpdate")]
    public static bool CameraLateUpdatePatch()
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerAnimationHandler), "SetAnimatorValues")]
    public static bool PlayerAnimationSetValuesPatch()
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerAnimationHandler), "HandleAnimationTarget")]
    public static bool PlayerAnimationHandleTargetPatch() // it gets worse
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerItems), "FixedUpdate")]
    public static bool PlayerItemsFixedUpdatePatch()
    {
        return !isFreeCam;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerItemsFake), "FixedUpdate")]
    public static bool PlayerItemsFakeFixedUpdatePatch()  // waiter waiter more <early return prefixes> please
    {
        return !isFreeCam;
    }
}

public class FreeCam : MonoBehaviour
{
    public GameObject head;
    public GameObject text;
    public GameObject text2electricboogaloo; // iamgoinginsaneiamgoinginsaneiamgoinginsane
    public GameObject hud;

    public void Update()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.M)) Plugin.isFreeCam = !Plugin.isFreeCam;
            if (Input.GetKeyDown(KeyCode.Comma)) Plugin.isHud = !Plugin.isHud;

            if (head == null) head = Player.localPlayer.transform.Find("Guy_W_Finger/Cube.001").gameObject;
            if (text == null) text = Player.localPlayer.transform.Find("HeadPosition/FACE").gameObject;
            if (text2electricboogaloo == null) text2electricboogaloo = text.transform.GetChild(0).gameObject;
            if (hud == null) hud = GameObject.Find("HelmetUI");

            hud.SetActive(Plugin.isHud);

            if (Plugin.isFreeCam)
            {
                head.layer = 0;
                text.layer = 0;
                text2electricboogaloo.layer = 0;
                Camera.main.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 0.15f);
                Camera.main.transform.rotation = Quaternion.LookRotation(Player.localPlayer.data.lookDirection);

            }
            else
            {
                head.layer = 29;
                text.layer = 29;
                text2electricboogaloo.layer = 29;
                Camera.main.transform.position = Player.localPlayer.refs.cameraPos.position;
                Camera.main.transform.rotation = Player.localPlayer.refs.cameraPos.rotation;
            }
        }
        catch { } // error handling? checking if player is null? nah just wrap it in a try catch block
    }
}