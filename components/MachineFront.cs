using Newtonsoft.Json;
using System.Collections.Generic;

namespace InfiniteItems.MachineParts
{
    public class MachineFront: EnvironmentObject, IClickable<int>
    {
        public InfiniteItemMachine linkedMachine;

        public void Clicked(int whichPlayer)
        {
            PlayerManager ply = Singleton<CoreGameManager>.Instance.GetPlayer(whichPlayer);

            if (!linkedMachine.Generating)
            {
                //Fill first slot
                if (linkedMachine.slotsFilled == 0 && ply.itm.items[ply.itm.selectedItem] != ply.itm.nothing)
                {
                    linkedMachine.SLOT_1_ITEM = ply.itm.items[ply.itm.selectedItem];
                    linkedMachine.ITEM_SLOT_1.sprite = ply.itm.items[ply.itm.selectedItem].itemSpriteLarge;
                    ply.itm.RemoveItem(ply.itm.selectedItem);
                    linkedMachine.MACHINE_AUDIO_1.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_ADD_ITEM"));
                    linkedMachine.slotsFilled = 1;
                }
                //Fill second slot and generate
                else if (linkedMachine.slotsFilled == 1 && ply.itm.items[ply.itm.selectedItem] != ply.itm.nothing)
                {
                    linkedMachine.SLOT_2_ITEM = ply.itm.items[ply.itm.selectedItem];
                    linkedMachine.ITEM_SLOT_2.sprite = ply.itm.items[ply.itm.selectedItem].itemSpriteLarge;
                    ply.itm.RemoveItem(ply.itm.selectedItem);
                    linkedMachine.MACHINE_AUDIO_1.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_ADD_ITEM"));
                    linkedMachine.MACHINE_AUDIO_2.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_READY"));
                    linkedMachine.slotsFilled = 2;
                }
                else if (linkedMachine.slotsFilled == 2)
                {
                   linkedMachine.MACHINE_AUDIO_1.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_PRESS"));
                   linkedMachine.Generating = true;
                   linkedMachine.GenerateNewItem();
                }
            }
        }
        public void ClickableSighted(int player) { }
        public void ClickableUnsighted(int player) { }
        public bool ClickableHidden() { return false; }
        public bool ClickableRequiresNormalHeight() { return true; }
    }
}