using MTM101BaldAPI;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Linq;

namespace InfiniteItems
{
    public class CustomInfiniteItemMachineFloor
    {
        public static CustomLevelObject GetFloor()
        {
            //Custom Faculty Room Group 
            var ML3 = (from x in Resources.FindObjectsOfTypeAll<SceneObject>()
                             where x.name == "MainLevel_3"
                             select x).First<SceneObject>();

            CustomLevelObject INF = (CustomLevelObject)ML3.levelObject;

            var MODDED_FACULTY = ML3.levelObject.roomGroup.First(x => x.name == "Faculty");
            MODDED_FACULTY.maxRooms = 50;
            MODDED_FACULTY.minRooms = 20;

            //No NPCs
            INF.potentialNPCs = new List<WeightedNPC>();
            INF.forcedNpcs = new NPC[0];
            INF.additionalNPCs = 0;
            INF.potentialBaldis = new WeightedNPC[0];

            //No posters
            INF.posterChance = 0;
            INF.minEvents = 0;
            INF.maxEvents = 0;

            //Very big
            INF.minSize = new IntVector2(32, 32);
            INF.maxSize = new IntVector2(96, 96);

            //Set lights
            INF.standardDarkLevel = new Color(19f / 255f, 19f / 255f, 85f / 255f, 1);
            INF.standardLightStrength = 6;
            INF.standardLightColor = new Color(255f / 255f, 235f / 255f, 135f / 255f, 1);

            //Set Items
            INF.potentialItems = new WeightedItemObject[] {
                new WeightedItemObject() { selection = InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Fire"), weight = 80 },
                new WeightedItemObject() { selection = InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Water"), weight = 80 },
                new WeightedItemObject() { selection = InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Earth"), weight = 80 },
                new WeightedItemObject() { selection = InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Wind"), weight = 80 },
                new WeightedItemObject() { selection = InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Life"), weight = 80 },

            };

            INF.forcedItems = new List<ItemObject>
            {
                InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Fire"),
                InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Water"),
                InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Earth"),
                InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Wind"),
                InfiniteItemsPlugin.Instance.assetManager.Get<ItemObject>("Life")
            };

            //Set Room Groups
            var rg = InfiniteItemsPlugin.Instance.ROOMGROUP;
            List<RoomGroup> rgs = INF.roomGroup.ToList();
            rg.minRooms = 10;
            rg.maxRooms = 30;
            rgs = new List<RoomGroup>() {
                rg,
                MODDED_FACULTY
            };
            INF.roomGroup = rgs.ToArray();

            //Set final stuff
            INF.MarkAsNeverUnload();
            INF.name = "CRT";

            //return
            return INF;
        }
    }
}