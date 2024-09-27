using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.Collections.Generic;
using UnityEngine;
using MTM101BaldAPI.Registers;
using System.Linq;
using System;

namespace InfiniteItems
{
    public class InfiniteItemMachineRoom
    {
        internal static class RoomInfo
        {
            internal const string roomName = "ItemizerRoom";
            internal const string folderName = "ItemizerRoom";
            internal const float r = 173f;
            internal const float g = 1f;
            internal const float b = 255f;
        }

        public static RoomGroup RegisterGroupAndRoom()
        {

            PosterObject LOCKER_POSTER = new PosterObject()
            {
                baseTexture = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "LockerPoster.png")
            };

            PosterData LOCKER_POSTER_DATA = new PosterData()
            {
                poster = LOCKER_POSTER
            };

            WeightedPosterObject WEIGHTED_LOCKER_POSTER = new WeightedPosterObject() { selection = LOCKER_POSTER, weight = 99999 };

            RoomCategory category = EnumExtensions.ExtendEnum<RoomCategory>(RoomInfo.folderName);

            RoomAsset ITEMIZER_ROOM = Resources.FindObjectsOfTypeAll<RoomAsset>().First(x => x.name == "Office_0");
            ITEMIZER_ROOM.category = category;
            ITEMIZER_ROOM.name = RoomInfo.roomName;
            ITEMIZER_ROOM.hasActivity = false;
            ITEMIZER_ROOM.spawnWeight = 10;
            ITEMIZER_ROOM.maxItemValue = 1;
            ITEMIZER_ROOM.activity = new ActivityData();
            ITEMIZER_ROOM.posterChance = 0.3f;
            ITEMIZER_ROOM.windowChance = 0f;
            ITEMIZER_ROOM.keepTextures = true;
            ITEMIZER_ROOM.posters = new List<WeightedPosterObject>() { WEIGHTED_LOCKER_POSTER };
            ITEMIZER_ROOM.roomFunction = new RoomFunction();
            ITEMIZER_ROOM.florTex = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "DiamondPlateFloor.png");
            ITEMIZER_ROOM.wallTex = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "WallWithMolding.png");
            ITEMIZER_ROOM.ceilTex = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "CeilingNoLight.png");
            ITEMIZER_ROOM.roomFunctionContainer = new RoomFunctionContainer();

            System.Random RANDOMIZER = new System.Random(System.Environment.TickCount);

            Vector3 MACHINE_TILE = new Vector3(
                ITEMIZER_ROOM.cells[RANDOMIZER.Next(0, ITEMIZER_ROOM.entitySafeCells.Count)].pos.x * 10,
                0,
                ITEMIZER_ROOM.cells[RANDOMIZER.Next(0, ITEMIZER_ROOM.entitySafeCells.Count)].pos.z * 10
            );

            RANDOMIZER = new System.Random(System.Environment.TickCount + 1);

            Vector3 LOCKER_TILE = new Vector3(
                ITEMIZER_ROOM.cells[RANDOMIZER.Next(0, ITEMIZER_ROOM.entitySafeCells.Count)].pos.x + 0.01f,
                0,
                ITEMIZER_ROOM.cells[RANDOMIZER.Next(0, ITEMIZER_ROOM.entitySafeCells.Count)].pos.z * RANDOMIZER.Next(0, 40)
            );

            RANDOMIZER = new System.Random(System.Environment.TickCount + 2);

            BasicObjectData MACHINE_PLACEMENT = new BasicObjectData()
            {
                prefab = InfiniteItemsPlugin.Instance.assetManager.Get<GameObject>("InfiniteItemMachine_Prefab").transform,
                position = new Vector3(25, 0, 25),
                rotation = Quaternion.Euler(0, RANDOMIZER.Next(-45, 45), 0),
                replaceable = false
            };

            RANDOMIZER = new System.Random(System.Environment.TickCount + 3);

            BasicObjectData LOCKER_PLACEMENT = new BasicObjectData()
            {
                prefab = InfiniteItemsPlugin.Instance.assetManager.Get<GameObject>("PurpleLocker_Prefab").transform,
                position = new Vector3(5, 0, 25),
                rotation = Quaternion.Euler(0, -90, 0),
                replaceable = false
            };

            ITEMIZER_ROOM.basicObjects = new List<BasicObjectData>() { MACHINE_PLACEMENT, LOCKER_PLACEMENT };

            StandardDoorMats doorMats = ObjectCreators.CreateDoorDataObject(RoomInfo.folderName + "Door",
                AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "Itemizer_Open.png"),
                AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "Itemizer_Closed.png")
            );

            ITEMIZER_ROOM.doorMats = doorMats;
            ITEMIZER_ROOM.color = new Color(RoomInfo.r / 255f, RoomInfo.g / 255f, RoomInfo.b / 255f);
            ITEMIZER_ROOM.mapMaterial = ObjectCreators.CreateMapTileShader(AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "MapBG_Infinite.png"));

            MTM101BaldiDevAPI.roomAssetMeta.Add(new RoomAssetMeta(InfiniteItemsPlugin.Instance.Info, ITEMIZER_ROOM));
            InfiniteItemsPlugin.Instance.assetManager.Add<RoomAsset>(RoomInfo.folderName, ITEMIZER_ROOM);

            RoomGroup ITEMIZER_ROOM_GROUP = Resources.FindObjectsOfTypeAll<LevelObject>().First(x => x.name == "Endless1").roomGroup.First(x => x.name == "Store");
            ITEMIZER_ROOM_GROUP.maxRooms = 1;
            ITEMIZER_ROOM_GROUP.floorTexture = new WeightedTexture2D[] { new WeightedTexture2D() { selection = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "DiamondPlateFloor.png") } };
            ITEMIZER_ROOM_GROUP.wallTexture = new WeightedTexture2D[] { new WeightedTexture2D() { selection = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "WallWithMolding.png") } };
            ITEMIZER_ROOM_GROUP.ceilingTexture = new WeightedTexture2D[] { new WeightedTexture2D() { selection = AssetLoader.TextureFromMod(InfiniteItemsPlugin.Instance, "Textures", "Room", "CeilingNoLight.png") } };
            ITEMIZER_ROOM_GROUP.potentialRooms = new WeightedRoomAsset[] { new WeightedRoomAsset() { selection = ITEMIZER_ROOM, weight = 1 } };
            ITEMIZER_ROOM_GROUP.stickToHallChance = 25;
            return ITEMIZER_ROOM_GROUP;
        }
    }
}