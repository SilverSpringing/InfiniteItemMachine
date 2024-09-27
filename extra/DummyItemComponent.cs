namespace InfiniteItems
{
    public class ITM_DummyItem : Item
    {
        public override bool Use(PlayerManager pm)
        {
            return false;
        }
    }
}