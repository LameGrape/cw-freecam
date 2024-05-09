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
    public const string PLUGIN_VERSION = "1.1.7";

    public static bool isFreeCam = false;
    public static bool canFreeCam = false;
    public static bool isHud = true;

    public static GameObject head;
    public static GameObject text;
    public static GameObject text2electricboogaloo; // iamgoinginsaneiamgoinginsaneiamgoinginsane
    public static GameObject hud;
    public static GameObject hat;

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
        hat = null;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MainCameraMovement), "LateUpdate")]
    public static bool CameraLateUpdatePatch()
    {
        if (SceneManager.GetActiveScene().name == "NewMainMenu") return true;

        if (Input.GetKeyDown(KeyCode.M)) isFreeCam = !isFreeCam;
        if (Input.GetKeyDown(KeyCode.Comma)) isHud = !isHud;

        if (head == null) head = Player.localPlayer.transform.Find("CharacterModel/HeadRenderer").gameObject;
        if (text == null) text = Player.localPlayer.transform.Find("HeadPosition/FACE").gameObject;
        if (text2electricboogaloo == null && text != null && text.transform.childCount > 0) text2electricboogaloo = text.transform.GetChild(0).gameObject;
        if (hud == null) hud = GameObject.Find("HelmetUI");
        try
        {
            if (hat == null) hat = Player.localPlayer.transform.Find("RigCreator/Rig/Armature/Hip/Torso/Head/HatPos").transform.GetChild(0).gameObject;
        }
        catch { } // no hat, just ignore

        hud.SetActive(isHud);

        if (isFreeCam)
        {
            head.layer = 0;
            text.layer = 0;
            if (text2electricboogaloo != null) text2electricboogaloo.layer = 0;
            if (hat != null) hat.layer = 0;
            Camera.main.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * 0.15f);
            Camera.main.transform.rotation = Quaternion.LookRotation(Player.localPlayer.data.lookDirection);

        }
        else
        {
            head.layer = 29;
            text.layer = 29;
            if (text2electricboogaloo != null) text2electricboogaloo.layer = 29;
            if (hat != null) hat.layer = 29;
            Camera.main.transform.position = Player.localPlayer.refs.cameraPos.position;
            Camera.main.transform.rotation = Player.localPlayer.refs.cameraPos.rotation;
        }

        return !isFreeCam;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerController), "Movement")]
    [HarmonyPatch(typeof(PlayerController), "SimpleMovement")]
    [HarmonyPatch(typeof(PlayerController), "TryJump")]
    [HarmonyPatch(typeof(PlayerAnimationHandler), "SetAnimatorValues")]
    [HarmonyPatch(typeof(PlayerAnimationHandler), "HandleAnimationTarget")]
    [HarmonyPatch(typeof(PlayerItems), "FixedUpdate")]
    [HarmonyPatch(typeof(PlayerItemsFake), "FixedUpdate")]
    public static bool EarlyReturns(MonoBehaviour __instance)
    {
        if (__instance.gameObject.GetComponent<Player>().ai) return true;
        else return !isFreeCam;
    }
}