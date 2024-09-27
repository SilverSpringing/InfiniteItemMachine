using UnityEngine;
using KoboldSharp;

namespace InfiniteItems
{
    public class InfiniteItemMachine : EnvironmentObject
    {
        public ItemObject GENERATED_ITEM, SLOT_1_ITEM, SLOT_2_ITEM;
        public int slotsFilled = 0;
        public PropagatedAudioManager MACHINE_AUDIO_1, MACHINE_AUDIO_2;
        public bool Generating;
        public SpriteRenderer ITEM_SLOT_1, ITEM_SLOT_2;
        public Sprite tL, tS;

        public void ResetMachine()
        {
            slotsFilled = 0;
            ITEM_SLOT_1.sprite = null;
            ITEM_SLOT_2.sprite = null;
            InfiniteItemsPlugin.Instance.client.ClearStory();
        }

        void Start()
        {
            InfiniteItemsPlugin.Instance.client = new KoboldClient("http://localhost:5001/v1");
        }

        public async void GenerateNewItem()
        {
            MACHINE_AUDIO_1.QueueAudio(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_INTRO"));

            string memory = "You are a helpful assistant that helps people to craft new things by combining two words into a new word. The most important rules that you absolutely must follow with every single answer are that you are not allowed to use the words "
            + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_1_ITEM.nameKey) + " and " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_2_ITEM.nameKey) +
            " as part of your answer and that you are only allowed to answer with one thing. DO NOT INCLUDE THE WORDS " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_1_ITEM.nameKey) + " and " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_2_ITEM.nameKey) + " as part of the answer!!!!! " +
            "The words " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_1_ITEM.nameKey) + " and " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_2_ITEM.nameKey) + " may NOT be part of the answer. No sentences, no phrases, no multiple words, no punctuation, no special characters, no numbers, no emojis, no URLs, no code, no commands, no programming. " +
            "THE ANSWER HAS TO ALWAYS BE A NOUN NO MATTER WHAT. No verbs, no pronouns, no adjectives, no adverbs. The order of the both words does not matter, both are equally important. " +
            "The answer has to be related to both words and the context of the words. The answer can either be a combination of the words, the role of one word in relation to the other, or an altered version of one word." +
            "Answers can be things, materials, people, companies, animals, occupations, food, places, objects, emotions, events, concepts, natural phenomena, body parts, vehicles, sports, " +
            "clothing, furniture, technology, buildings, technology, instruments, beverages, plants, academic subjects and everything else you can think of that is a noun.\n\n";

            //GIVE PROMPT
            GenParams GENERATION_INFO = new GenParams("Reply with the result of what would happen if you combine " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_1_ITEM.nameKey) + " and " + Singleton<LocalizationManager>.Instance.GetLocalizedText(SLOT_2_ITEM.nameKey) + ". The answer has to be related to both words and the context of the words and may not contain the words themselves.\n\n<|eot_id|><|start_header_id|>assistant<|end_header_id|>\n\n");
            GENERATION_INFO.MaxLength = 30;
            GENERATION_INFO.MaxContextLength = 4096;
            GENERATION_INFO.Temperature = 0.2f;
            GENERATION_INFO.TopP = 0.6f;
            GENERATION_INFO.TopK = 100;
            GENERATION_INFO.Memory = memory;

            //WAIT TO GENERATE
            try
            {
                ModelOutput MODEL_OUTPUT = await InfiniteItemsPlugin.Instance.client.Generate(GENERATION_INFO);
                GENERATED_ITEM = await ItemClass.CreateNewItem(MODEL_OUTPUT.Results[0].Text, "Placeholder");
                EndGeneration(true);
            }
            catch
            {
                EndGeneration(false);
            }
        }

        void EndGeneration(bool success)
        {
            MACHINE_AUDIO_1.FlushQueue(true);
            MACHINE_AUDIO_2.FlushQueue(true);

            if (success)
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.AddItem(GENERATED_ITEM);
                MACHINE_AUDIO_1.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_GEN_SUCCESS"));
            }
            else
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.AddItem(SLOT_1_ITEM);
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.AddItem(SLOT_2_ITEM);
                MACHINE_AUDIO_1.PlaySingle(InfiniteItemsPlugin.Instance.assetManager.Get<SoundObject>("MACHINE_GEN_FAIL"));
            }

            Generating = false;
            ResetMachine();
        }
    }
}