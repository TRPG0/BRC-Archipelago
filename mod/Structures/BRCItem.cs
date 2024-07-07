namespace Archipelago.BRC.Structures
{
    public class BRCItem : AItem
    {
        public BRCType type;
        public bool received;
    }

    public enum BRCType
    {
        Music,
        GraffitiM,
        GraffitiL,
        GraffitiXL,
        Skateboard,
        InlineSkates,
        BMX,
        Character,
        Outfit,
        REP,
        Camera
    }
}
