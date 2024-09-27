using UnityEngine;

namespace InfiniteItems.MachineParts
{
    public class MachineBack : EnvironmentObject, IClickable<int>
    {
        public InfiniteItemMachine linkedMachine;
        private float unpressTime;
        private bool restored;

        public void Update()
        {
            if (unpressTime >= 0)
            {
                unpressTime -= Time.deltaTime;
            }
            else if (!restored)
            {
                linkedMachine.gameObject.GetComponent<MeshRenderer>().materials = new Material[] { 
                    InfiniteItemsPlugin.Instance.assetManager.Get<Material>("InfiniteItem_Front"),
                    InfiniteItemsPlugin.Instance.assetManager.Get<Material>("InfiniteItem_Sides"),
                    InfiniteItemsPlugin.Instance.assetManager.Get<Material>("InfiniteItem_ResetButton")
                };
                restored = true;
            }
        }

        public void Clicked(int whichPlayer)
        {
            PlayerManager ply = Singleton<CoreGameManager>.Instance.GetPlayer(whichPlayer);

            if (linkedMachine.slotsFilled != 0)
            {
                if (linkedMachine.slotsFilled == 1)
                {
                    ply.itm.AddItem(linkedMachine.SLOT_1_ITEM);
                }
                else
                {
                    ply.itm.AddItem(linkedMachine.SLOT_1_ITEM);
                    ply.itm.AddItem(linkedMachine.SLOT_2_ITEM);
                }

                linkedMachine.ResetMachine();

                restored = false;
                linkedMachine.gameObject.GetComponent<MeshRenderer>().materials = new Material[] {
                    InfiniteItemsPlugin.Instance.assetManager.Get<Material>("InfiniteItem_Front"),
                    InfiniteItemsPlugin.Instance.assetManager.Get<Material>("InfiniteItem_Sides"),
                    InfiniteItemsPlugin.Instance.assetManager.Get<Material>("InfiniteItem_ResetButton_Pressed")
                };
                unpressTime = 0.2f;

                linkedMachine.MACHINE_AUDIO_1.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_PRESS"));
            }
        }
        public void ClickableSighted(int player) { }
        public void ClickableUnsighted(int player) { }
        public bool ClickableHidden() { return false; }
        public bool ClickableRequiresNormalHeight() { return true; }
    }
}