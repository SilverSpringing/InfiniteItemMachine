using UnityEngine;
using System;
using System.Threading.Tasks;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace InfiniteItems
{ 
	public class ItemClass : MonoBehaviour
    {
        public async static Task<ItemObject> CreateNewItem(string Name, string Desc)
		{
			string GenItemName = "Placeholder";
			string GenItemDesc = "Placeholder\nNo description.";

			if (Name != null) GenItemName = Name;
			if (Desc != null) GenItemDesc = Desc;

			ItemObject GENERATED_ITEM = ScriptableObject.CreateInstance<ItemObject>();

			var ItemEnum = Items.None;

			//if the item already exists
			if (File.Exists(Path.Combine(AssetLoader.GetModPath(InfiniteItemsPlugin.Instance), "ItemCache", Regex.Replace(GenItemName, "<>:\"/\\|*?", "") + ".json")))
			{
				LoadFromItemCache(GenItemName);
			}
			else //if the item DOES NOT EXIST
            {
				Debug.Log("Extending enum");
				ItemEnum = EnumExtensions.ExtendEnum<Items>(GENERATED_ITEM.name);
				GENERATED_ITEM.itemType = ItemEnum;
				GENERATED_ITEM.nameKey = GenItemName;
				GENERATED_ITEM.descKey = GenItemDesc;
				GENERATED_ITEM.name = "INF_ITEM_" + GenItemName;
				GENERATED_ITEM.price = 0;

				Debug.Log("Creating dummy object");
				GameObject dummyObj = new GameObject();
				dummyObj.SetActive(false);
				Item dummyItem = (Item)dummyObj.AddComponent(typeof(ITM_DummyItem));
				dummyItem.name = "Obj" + GENERATED_ITEM.name;
				GENERATED_ITEM.item = dummyItem;
				Destroy(dummyObj);

				Debug.Log("Fetching icon");
				IconFetcherClient FetcherClient = new IconFetcherClient(GENERATED_ITEM.name);
				Texture2D ItemTexture = await FetcherClient.GetRandomTexture();

				Debug.Log("Creating icons");
				GENERATED_ITEM.itemSpriteLarge = Sprite.Create(ItemTexture, new Rect(0.0f, 0.0f, 64, 64), new Vector2(0.5f, 0.5f), 100.0f);
                GENERATED_ITEM.itemSpriteSmall = Sprite.Create(ItemTexture, new Rect(0.0f, 0.0f, 32, 32), new Vector2(0.5f, 0.5f), 100.0f);

				Debug.Log("Writing to cache");
				WriteToItemCache(GENERATED_ITEM, ItemTexture);
			}

            return GENERATED_ITEM;
		}

		public static ItemObject LoadFromItemCache(string ItemName)
		{
			string path = Path.Combine(AssetLoader.GetModPath(InfiniteItemsPlugin.Instance), "ItemCache", Regex.Replace(ItemName, "<>:\"/\\|*?", "") + ".json");
			ItemJSONData JSON_DATA = new ItemJSONData();

			Debug.Log("Attempting to load from " + path);

			using (StreamReader file = File.OpenText(path))
			using (JsonTextReader reader = new JsonTextReader(file))
			{
				JsonSerializer serializer = new JsonSerializer();
				JSON_DATA = (ItemJSONData)serializer.Deserialize(file, typeof(ItemJSONData));
			}

			ItemObject itemNew = ScriptableObject.CreateInstance<ItemObject>();
			itemNew.nameKey = JSON_DATA.Name;
			itemNew.descKey = JSON_DATA.Description;

			byte[] imageData = Convert.FromBase64String(JSON_DATA.IconData);

			Texture2D tex = new Texture2D(64, 64, TextureFormat.ARGB32, false);
			tex.LoadRawTextureData(imageData);
			tex.Apply();

			Debug.Log("Creating icons");
			itemNew.itemSpriteLarge = Sprite.Create(tex, new Rect(0.0f, 0.0f, 64, 64), new Vector2(0.5f, 0.5f), 100.0f);
			itemNew.itemSpriteSmall = Sprite.Create(tex, new Rect(0.0f, 0.0f, 32, 32), new Vector2(0.5f, 0.5f), 100.0f);

			return itemNew;
		}

		public static void WriteToItemCache(ItemObject item, Texture2D tex)
        {
			ItemJSONData ITEM_JSON = new ItemJSONData(item.nameKey, "", Convert.ToBase64String(tex.EncodeToPNG()));

			string path = Path.Combine(AssetLoader.GetModPath(InfiniteItemsPlugin.Instance), "ItemCache", Regex.Replace(item.nameKey, "<>:\"/\\|*?", "") + ".json");

			File.WriteAllText(path, ITEM_JSON.ToString());

			using (StreamWriter file = File.CreateText(path))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Serialize(file, ITEM_JSON);
			}

			Debug.Log("Wrote item data to " + path);
		}
	}
}
