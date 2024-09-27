using BepInEx;
using Dummiesman;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Diagnostics;
using MTM101BaldAPI.ObjectCreation;
using System.Collections;
using KoboldSharp;
using BepInEx.Configuration;
using MTM101BaldAPI.Reflection;

namespace InfiniteItems
{
    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "silverspringing.baldiplus.endlessitems";
        public const string PLUGIN_NAME = "Endless Items";
        public const string PLUGIN_VERSION = "1.0.0.0";
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class InfiniteItemsPlugin : BaseUnityPlugin
    {
        public Mode CREATIVE_MODE;

        public AssetManager assetManager = new AssetManager();
        public static InfiniteItemsPlugin Instance;
        public KoboldClient client;
        public RoomGroup ROOMGROUP;

        public static ConfigEntry<bool> autoStartKobold;
        public static ConfigEntry<bool> pauseWhenOpening;

        IEnumerator StartKobold()
        {
            yield return "Starting KoboldCPP...";

            string filename = Path.Combine(AssetLoader.GetModPath(this), "Kobold", "run.bat");
            string parameters = $"/k \"{filename}\"";
            Process.Start("cmd", parameters);

            yield break;
        }

        void CreateElementLockerPrefab()
        {
            //Material[] BRUH = Resources.FindObjectsOfTypeAll<Material>();
            //foreach (Material m in BRUH) { UnityEngine.Debug.Log(m.name); }

            OBJLoader LOCKER_IMPORT = new OBJLoader();
            GameObject PURPLE_LOCKER_CLOSED = LOCKER_IMPORT.Load(Path.Combine(AssetLoader.GetModPath(this), "Models", "PurpleLocker", "PurpleLocker_Closed.obj"), Path.Combine(AssetLoader.GetModPath(this), "Models", "PurpleLocker", "PurpleLocker_Closed.mtl"));
            GameObject PURPLE_LOCKER_OPEN = LOCKER_IMPORT.Load(Path.Combine(AssetLoader.GetModPath(this), "Models", "PurpleLocker", "PurpleLocker_Open.obj"), Path.Combine(AssetLoader.GetModPath(this), "Models", "PurpleLocker", "PurpleLocker_Open.mtl"));

            GameObject PURPLE_LOCKER_PREFAB = Instantiate(Resources.FindObjectsOfTypeAll<StorageLocker>().ToList().First()).gameObject;
            Destroy(PURPLE_LOCKER_PREFAB.GetComponent<StorageLocker>());

            PurpleLocker PURPLE_LOCKER_COMPONENT = PURPLE_LOCKER_PREFAB.AddComponent<PurpleLocker>();
            PURPLE_LOCKER_PREFAB.name = "PurpleLocker";

            PURPLE_LOCKER_COMPONENT.SetMeshes(PURPLE_LOCKER_CLOSED.GetComponentInChildren<MeshFilter>().mesh, PURPLE_LOCKER_OPEN.GetComponentInChildren<MeshFilter>().mesh);

            //CREATE MATERIAL FOR LOCKER
            Material LOCKER_MATERIAL = Instantiate(Resources.FindObjectsOfTypeAll<Material>().ToList().Find(x => x.name == "Locker_Green"));
            LOCKER_MATERIAL.name = "PurpleLocker_Material";
            LOCKER_MATERIAL.SetMainTexture(AssetLoader.TextureFromMod(this, "Textures", "PurpleLocker", "Locker_Purple_Combined.png"));

            //DESTROY ITEM SLOTS
            Destroy(PURPLE_LOCKER_PREFAB.transform.GetChild(1).gameObject);
            Destroy(PURPLE_LOCKER_PREFAB.transform.GetChild(2).gameObject);
            Destroy(PURPLE_LOCKER_PREFAB.transform.GetChild(3).gameObject);

            //SET LOCKER MESH
            PURPLE_LOCKER_PREFAB.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh = PURPLE_LOCKER_CLOSED.GetComponentInChildren<MeshFilter>().mesh;
            //PURPLE_LOCKER_PREFAB.transform.localScale = new Vector3(0.4f, 1, 1);

            foreach (MeshRenderer mr in PURPLE_LOCKER_PREFAB.transform.GetComponentsInChildren<MeshRenderer>()) mr.materials = new Material[] { LOCKER_MATERIAL };

            //CREATE SOUNDS FOR LOCKER
            SoundObject LOCKER_OPEN = ScriptableObject.CreateInstance<SoundObject>();
            LOCKER_OPEN.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "LockerOpen.wav"));
            LOCKER_OPEN.subtitle = false;
            LOCKER_OPEN.soundType = SoundType.Effect;
            LOCKER_OPEN.name = "LockerOpen";

            SoundObject LOCKER_CLOSE = ScriptableObject.CreateInstance<SoundObject>();
            LOCKER_CLOSE.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "LockerClose.wav"));
            LOCKER_CLOSE.subtitle = false;
            LOCKER_CLOSE.soundType = SoundType.Effect;
            LOCKER_CLOSE.name = "LockerClose";

            assetManager.Add<SoundObject>("PURPLE_LOCKER_CLOSE", LOCKER_CLOSE);
            assetManager.Add<SoundObject>("PURPLE_LOCKER_OPEN", LOCKER_OPEN);

            PURPLE_LOCKER_PREFAB.ConvertToPrefab(true);
            assetManager.Add<GameObject>("PurpleLocker_Prefab", PURPLE_LOCKER_PREFAB);

            Destroy(PURPLE_LOCKER_CLOSED);
            Destroy(PURPLE_LOCKER_OPEN);
        }

        void CreateMachinePrefab()
        {
/*            foreach (LevelObject s in Resources.FindObjectsOfTypeAll<LevelObject>())
            {
                if (s.roomGroup != null)
                {
                    foreach (RoomGroup r in s.roomGroup)
                    {
                        UnityEngine.Debug.Log(r.name);
                    }
                }
            }*/

            //IMPORT OBJ
            OBJLoader OBJ_IMPORT = new OBJLoader();
            GameObject MACHINE_BASE_MODEL = OBJ_IMPORT.Load(Path.Combine(AssetLoader.GetModPath(this), "Models", "InfiniteItemMachineV2.obj"), Path.Combine(AssetLoader.GetModPath(this), "Models", "InfiniteItemMachineV2.mtl"));

            //CREATE MATERIAL FOR FRONT
            Material ITEM_MACHINE_MATERIAL_FRONT = Instantiate(Resources.FindObjectsOfTypeAll<Material>().ToList().Find(x => x.name == "Bookshelf"));
            ITEM_MACHINE_MATERIAL_FRONT.name = "InfiniteItemMachine_Material_Front";
            ITEM_MACHINE_MATERIAL_FRONT.mainTextureScale = new Vector2(1, 1);
            ITEM_MACHINE_MATERIAL_FRONT.SetMainTexture(AssetLoader.TextureFromMod(this, "Textures", "InfiniteItemMachine_Front.png"));
            ITEM_MACHINE_MATERIAL_FRONT.SetTexture("_LightGuide", AssetLoader.TextureFromMod(this, "Textures", "InfiniteItemMachine_Front_LightGuide.png"));

            //CREATE MATERIAL FOR SIDE
            Material ITEM_MACHINE_MATERIAL_SIDES = Instantiate(ITEM_MACHINE_MATERIAL_FRONT);
            ITEM_MACHINE_MATERIAL_SIDES.name = "InfiniteItemMachine_Material_Side";
            ITEM_MACHINE_MATERIAL_SIDES.mainTextureScale = new Vector2(1, 1);
            ITEM_MACHINE_MATERIAL_SIDES.SetMainTexture(AssetLoader.TextureFromMod(this, "Textures", "InfiniteItemMachine_Side.png"));
            ITEM_MACHINE_MATERIAL_SIDES.SetTexture("_LightGuide", null);

            //CREATE MATERIAL FOR BACK
            Material ITEM_MACHINE_MATERIAL_RESET_1 = Instantiate(ITEM_MACHINE_MATERIAL_SIDES);
            ITEM_MACHINE_MATERIAL_FRONT.mainTextureScale = new Vector2(1, 1);
            ITEM_MACHINE_MATERIAL_RESET_1.name = "InfiniteItemMachine_Material_Reset";
            ITEM_MACHINE_MATERIAL_RESET_1.SetMainTexture(AssetLoader.TextureFromMod(this, "Textures", "InfiniteItemMachine_Back.png"));

            //CREATE MATERIAL FOR BACK PRESSED
            Material ITEM_MACHINE_MATERIAL_RESET_2 = Instantiate(ITEM_MACHINE_MATERIAL_SIDES);
            ITEM_MACHINE_MATERIAL_RESET_2.mainTextureScale = new Vector2(1, 1);
            ITEM_MACHINE_MATERIAL_RESET_2.name = "InfiniteItemMachine_Material_Reset_Pressed";
            ITEM_MACHINE_MATERIAL_RESET_2.SetMainTexture(AssetLoader.TextureFromMod(this, "Textures", "InfiniteItemMachine_Back_Press.png"));

            assetManager.Add<Material>("InfiniteItem_Front", ITEM_MACHINE_MATERIAL_FRONT);
            assetManager.Add<Material>("InfiniteItem_Sides", ITEM_MACHINE_MATERIAL_SIDES);
            assetManager.Add<Material>("InfiniteItem_ResetButton", ITEM_MACHINE_MATERIAL_RESET_1);
            assetManager.Add<Material>("InfiniteItem_ResetButton_Pressed", ITEM_MACHINE_MATERIAL_RESET_2);

            //GET MESH FILTERS
            GameObject TMP_MACHINE_BASE = Instantiate(Resources.FindObjectsOfTypeAll<SodaMachine>().ToList().Find(x => x.name == "SodaMachine")).gameObject;
            Destroy(TMP_MACHINE_BASE.GetComponent<SodaMachine>());
            TMP_MACHINE_BASE.gameObject.transform.localScale = new Vector3(1, 1, 1);
            TMP_MACHINE_BASE.name = "InfiniteItemMachine";
            TMP_MACHINE_BASE.AddComponent<MeshCollider>().sharedMesh = MACHINE_BASE_MODEL.GetComponentInChildren<MeshFilter>().mesh;
            TMP_MACHINE_BASE.GetComponent<MeshFilter>().mesh = MACHINE_BASE_MODEL.GetComponentInChildren<MeshFilter>().mesh;
            TMP_MACHINE_BASE.GetComponent<MeshRenderer>().materials = new Material[] { ITEM_MACHINE_MATERIAL_FRONT, ITEM_MACHINE_MATERIAL_SIDES, ITEM_MACHINE_MATERIAL_RESET_1 };
            InfiniteItemMachine MACHINE_BASE_COMPONENT = TMP_MACHINE_BASE.AddComponent<InfiniteItemMachine>();

            GameObject MACHINE_INPUT_HITBOX = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MACHINE_INPUT_HITBOX.name = "MachineInputHitbox";
            MACHINE_INPUT_HITBOX.transform.SetParent(TMP_MACHINE_BASE.transform);
            MACHINE_INPUT_HITBOX.transform.localScale = new Vector3(6.233f, 0.438f, 2.501f);
            MACHINE_INPUT_HITBOX.transform.localPosition = new Vector3(0f, 4.748f, -4.598f);
            MACHINE_INPUT_HITBOX.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            MACHINE_INPUT_HITBOX.AddComponent<MachineParts.MachineFront>().linkedMachine = MACHINE_INPUT_HITBOX.transform.parent.GetComponent<InfiniteItemMachine>();
            Destroy(MACHINE_INPUT_HITBOX.GetComponent<MeshRenderer>());
            MACHINE_INPUT_HITBOX.GetComponent<BoxCollider>().isTrigger = true;

            GameObject MACHINE_RESET_HITBOX = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MACHINE_RESET_HITBOX.name = "MachineResetHitbox";
            MACHINE_RESET_HITBOX.transform.SetParent(TMP_MACHINE_BASE.transform);
            MACHINE_RESET_HITBOX.transform.localScale = new Vector3(2.605f, 2.605f, 1.303f);
            MACHINE_RESET_HITBOX.transform.localPosition = new Vector3(0f, 4.7f, 4.291f);
            MACHINE_RESET_HITBOX.AddComponent<MachineParts.MachineBack>().linkedMachine = MACHINE_RESET_HITBOX.transform.parent.GetComponent<InfiniteItemMachine>();
            Destroy(MACHINE_RESET_HITBOX.GetComponent<MeshRenderer>());
            MACHINE_RESET_HITBOX.GetComponent<BoxCollider>().isTrigger = true;

            //CREATE ITEM SLOTS
            MACHINE_BASE_COMPONENT.ITEM_SLOT_1 = new GameObject("InputItemSprite1", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            MACHINE_BASE_COMPONENT.ITEM_SLOT_1.transform.SetParent(TMP_MACHINE_BASE.transform, false);
            MACHINE_BASE_COMPONENT.ITEM_SLOT_1.material = Resources.FindObjectsOfTypeAll<Material>().ToList().Find(x => x.name == "Lit_SpriteStandard_NoBillboard");
            MACHINE_BASE_COMPONENT.ITEM_SLOT_1.transform.position = new Vector3(-2f, 4.8f, -4.815f);

            MACHINE_BASE_COMPONENT.ITEM_SLOT_2 = new GameObject("InputItemSprite2", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            MACHINE_BASE_COMPONENT.ITEM_SLOT_2.transform.SetParent(TMP_MACHINE_BASE.transform, false);
            MACHINE_BASE_COMPONENT.ITEM_SLOT_2.material = Resources.FindObjectsOfTypeAll<Material>().ToList().Find(x => x.name == "Lit_SpriteStandard_NoBillboard");
            MACHINE_BASE_COMPONENT.ITEM_SLOT_2.transform.position = new Vector3(2f, 4.8f, -4.815f);

            //ADD PROPAGATED AUDIO
            MACHINE_BASE_COMPONENT.MACHINE_AUDIO_1 = TMP_MACHINE_BASE.AddComponent<PropagatedAudioManager>();
            MACHINE_BASE_COMPONENT.MACHINE_AUDIO_2 = TMP_MACHINE_BASE.AddComponent<PropagatedAudioManager>();

            //CONVERT TO PREFAB
            TMP_MACHINE_BASE.ConvertToPrefab(true);
            assetManager.Add<GameObject>("InfiniteItemMachine_Prefab", TMP_MACHINE_BASE);

            Destroy(MACHINE_BASE_MODEL);

        }

        void RegisterInfiniteItemMachine()
        {
            CreateMachinePrefab();
            CreateElementLockerPrefab();

            ROOMGROUP = InfiniteItemMachineRoom.RegisterGroupAndRoom();

            SoundObject GEN_SUCCESS = ScriptableObject.CreateInstance<SoundObject>();
            GEN_SUCCESS.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "Gen_Success.wav"));
            GEN_SUCCESS.subtitle = true;
            GEN_SUCCESS.soundKey = "Gen_Success";
            GEN_SUCCESS.subDuration = 10;
            GEN_SUCCESS.soundType = SoundType.Effect;
            GEN_SUCCESS.name = "GenSuccess";

            SoundObject GEN_FAIL = ScriptableObject.CreateInstance<SoundObject>();
            GEN_FAIL.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "Gen_Fail.wav"));
            GEN_FAIL.subtitle = true;
            GEN_FAIL.soundKey = "Gen_Fail";
            GEN_FAIL.subDuration = 10;
            GEN_FAIL.soundType = SoundType.Effect;
            GEN_FAIL.name = "GenFail";

            SoundObject PRESS = ScriptableObject.CreateInstance<SoundObject>();
            PRESS.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "Button_Press.wav"));
            PRESS.subtitle = false;
            PRESS.soundType = SoundType.Effect;
            PRESS.name = "Press";

            SoundObject UNPRESS = ScriptableObject.CreateInstance<SoundObject>();
            UNPRESS.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "Button_Unpress.wav"));
            UNPRESS.subtitle = false;
            UNPRESS.soundType = SoundType.Effect;
            UNPRESS.name = "Unpress";

            SoundObject BUZZ = ScriptableObject.CreateInstance<SoundObject>();
            BUZZ.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "Buzz.wav"));
            BUZZ.subtitle = true;
            BUZZ.subDuration = 3;
            BUZZ.soundKey = "Gen_Ready";
            BUZZ.soundType = SoundType.Effect;
            BUZZ.name = "Buzz";

            SoundObject GENERATING_SFX = ScriptableObject.CreateInstance<SoundObject>();
            GENERATING_SFX.soundClip = AssetLoader.AudioClipFromFile(Path.Combine(AssetLoader.GetModPath(this), "Audio", "MachineTHEQUEUESYSTEMISBROKEN.mp3"));
            GENERATING_SFX.subtitle = true;
            GENERATING_SFX.soundKey = "Gen_Busy";
            GENERATING_SFX.soundType = SoundType.Effect;
            GENERATING_SFX.name = "Generating";

            assetManager.Add<SoundObject>("MACHINE_INTRO", GENERATING_SFX);
            assetManager.Add<SoundObject>("MACHINE_PRESS", PRESS);
            assetManager.Add<SoundObject>("MACHINE_GEN_SUCCESS", GEN_SUCCESS);
            assetManager.Add<SoundObject>("MACHINE_GEN_FAIL", GEN_FAIL);
            assetManager.Add<SoundObject>("MACHINE_ADD_ITEM", UNPRESS);
            assetManager.Add<SoundObject>("MACHINE_READY", BUZZ);

            //load locker stuff
            assetManager.Add<Sprite>("NewBrowserMain", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "Browser", "BrowserMain.png"), 26f));
            assetManager.Add<Sprite>("BrowserArrow", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "Browser", "PageArrow.png"), 26f));

            //set temp icons
            assetManager.Add<Sprite>("TEMP_LARGE", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "Placeholder_Large.png"), 26f));
            assetManager.Add<Sprite>("TEMP_SMALL", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "Placeholder_Small.png"), 26f));

            //set elements
            assetManager.Add<Sprite>("Fire_Large", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Fire_Large.png"), 26f));
            assetManager.Add<Sprite>("Fire_Small", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Fire_Small.png"), 26f));
            assetManager.Add<Sprite>("Water_Large", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Water_Large.png"), 26f));
            assetManager.Add<Sprite>("Water_Small", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Water_Small.png"), 26f));
            assetManager.Add<Sprite>("Earth_Large", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Earth_Large.png"), 26f));
            assetManager.Add<Sprite>("Earth_Small", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Earth_Small.png"), 26f));
            assetManager.Add<Sprite>("Wind_Large", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Wind_Large.png"), 26f));
            assetManager.Add<Sprite>("Wind_Small", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Wind_Small.png"), 26f));
            assetManager.Add<Sprite>("Life_Large", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Life_Large.png"), 26f));
            assetManager.Add<Sprite>("Life_Small", AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromMod(this, "Sprites", "BaseElements", "Life_Small.png"), 26f));

            ItemObject fire = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_Fire", "Desc_Fire")
                .SetSprites(assetManager.Get<Sprite>("Fire_Small"), assetManager.Get<Sprite>("Fire_Large"))
                .SetEnum("Fire")
                .SetShopPrice(50)
                .SetGeneratorCost(20)
                .SetItemComponent<ITM_DummyItem>()
                .SetMeta(ItemFlags.Persists, new string[0])
            .Build();

            ItemObject water = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_Water", "Desc_Water")
                .SetSprites(assetManager.Get<Sprite>("Water_Small"), assetManager.Get<Sprite>("Water_Large"))
                .SetEnum("Water")
                .SetShopPrice(50)
                .SetGeneratorCost(20)
                .SetItemComponent<ITM_DummyItem>()
                .SetMeta(ItemFlags.Persists, new string[0])
            .Build();

            ItemObject earth = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_Earth", "Desc_Earth")
                .SetSprites(assetManager.Get<Sprite>("Earth_Small"), assetManager.Get<Sprite>("Earth_Large"))
                .SetEnum("Water")
                .SetShopPrice(50)
                .SetGeneratorCost(20)
                .SetItemComponent<ITM_DummyItem>()
                .SetMeta(ItemFlags.Persists, new string[0])
            .Build();

            ItemObject wind = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_Wind", "Desc_Wind")
                .SetSprites(assetManager.Get<Sprite>("Wind_Small"), assetManager.Get<Sprite>("Wind_Large"))
                .SetEnum("Water")
                .SetShopPrice(50)
                .SetItemComponent<ITM_DummyItem>()
                .SetGeneratorCost(20)
                .SetMeta(ItemFlags.Persists, new string[0])
            .Build();

            ItemObject life = new ItemBuilder(Info)
                .SetNameAndDescription("Itm_Life", "Desc_Life")
                .SetSprites(assetManager.Get<Sprite>("Life_Small"), assetManager.Get<Sprite>("Life_Large"))
                .SetEnum("Life")
                .SetShopPrice(100)
                .SetItemComponent<ITM_DummyItem>()
                .SetGeneratorCost(40)
                .SetMeta(ItemFlags.Persists, new string[0])
            .Build();

            assetManager.Add<ItemObject>("Fire", fire);
            assetManager.Add<ItemObject>("Water", water);
            assetManager.Add<ItemObject>("Earth", earth);
            assetManager.Add<ItemObject>("Wind", wind);
            assetManager.Add<ItemObject>("Life", life);

        }
        void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAllConditionals();
            Instance = this;

            CREATIVE_MODE = EnumExtensions.ExtendEnum<Mode>("Creative");

            LoadingEvents.RegisterOnAssetsLoaded(Info, RegisterInfiniteItemMachine, false);

            GeneratorManagement.Register(this, GenerationModType.Base, AddExtraStuff);

            autoStartKobold = Instance.Config.Bind("InfiniteItemMachine", "AutoStartKobold", true, "If enabled, Kobold will start upon launching Baldi's Basics Plus.");
            pauseWhenOpening = Instance.Config.Bind("InfiniteItemMachine", "PauseWhenInPurpleLocker", true, "If enabled, time will stop when accessing the Purple locker.");

            if (autoStartKobold.Value == true) StartCoroutine(StartKobold());
        }

        void AddExtraStuff(string floorName, int floorNumber, CustomLevelObject floorObject)
        {
            if (floorName == "END")
            {
                List<RoomGroup> rmGL = floorObject.roomGroup.ToList();
                rmGL.Insert(0, ROOMGROUP);
                floorObject.roomGroup = rmGL.ToArray();

                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Fire"), weight = 80 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Water"), weight = 80 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Earth"), weight = 80 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Wind"), weight = 80 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Life"), weight = 65 }).ToArray();
                floorObject.MarkAsNeverUnload();
            }
            else if (floorName.StartsWith("F"))
            {
                List<RoomGroup> rmGL = floorObject.roomGroup.ToList();
                rmGL.Insert(0, ROOMGROUP);
                floorObject.roomGroup = rmGL.ToArray();

                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Water"), weight = 70 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Earth"), weight = 70 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Wind"), weight = 70 }).ToArray();
                floorObject.potentialItems = floorObject.potentialItems.AddItem(new WeightedItemObject() { selection = assetManager.Get<ItemObject>("Life"), weight = 40 }).ToArray();
                floorObject.MarkAsNeverUnload();
            }
        }
    }
}