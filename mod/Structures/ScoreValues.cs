namespace Archipelago.Structures
{
    public class ScoreValues : Values
    {
        public int hardValue;
        public int veryHardValue;
        public int extremeValue;

        public ScoreValues(int oldValue, int hardValue, int veryHardValue, int extremeValue)
        {
            this.oldValue = oldValue;
            this.hardValue = hardValue;
            this.veryHardValue = veryHardValue;
            this.extremeValue = extremeValue;
        }
    }
}
