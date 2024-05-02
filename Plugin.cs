using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace freecam;

[ContentWarningPlugin(Plugin.PLUGIN_GUID, Plugin.PLUGIN_VERSION, false)]
[BepInPlugin(Plugin.PLUGIN_GUID, "freecam", Plugin.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public const string PLUGIN_GUID = "raisin.plugin.freecam";
    public const string PLUGIN_VERSION = "1.1.5";

    public static bool isFreeCam = false;
    public static bool canFreeCam = false;
    public static bool isHud = true;

    public static GameObject head;
    public static GameObject text;
    public static GameObject text2electricboogaloo; // iamgoinginsaneiamgoinginsaneiamgoinginsane
    public static GameObject hud;

    private void Awake()
    {
        Logger.LogInfo($"Loaded Freecam {PLUGIN_VERSION}");

        SceneManager.activeSceneChanged += SceneChanged;

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    private void SceneChanged(Scene current, Scene next)
    {
        canFreeCam = next.name != "NewMainMenu";
        isFreeCam = false;
        head = null;
        text = null;
        text2electricboogaloo = null;
        hud = null;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MainCameraMovement), "LateUpdate")]
    public static bool CameraLateUpdatePatch()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.M)) isFreeCam = !isFreeCam;
            if (Input.GetKeyDown(KeyCode.Comma)) isHud = !isHud;

            if (head == null) head = Player.localPlayer.transform.Find("CharacterModel/HeadRenderer").gameObject;
            if (text == null) text = Player.localPlayer.transform.Find("HeadPosition/FACE").gameObject;
            if (text2electricboogaloo == null) text2electricboogaloo = text.transform.GetChild(0).gameObject;
            if (hud == null) hud = GameObject.Find("HelmetUI");

            hud.SetActive(isHud);

            if (isFreeCam)
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

        return !isFreeCam;
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