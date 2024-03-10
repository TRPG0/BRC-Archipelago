namespace Archipelago.BRC.Structures
{
    public class ScoreValues : Values
    {
        public int mediumValue;
        public int hardValue;
        public int veryHardValue;
        public int extremeValue;

        public ScoreValues(int oldValue, int mediumValue, int hardValue, int veryHardValue, int extremeValue)
        {
            this.oldValue = oldValue;
            this.mediumValue = mediumValue;
            this.hardValue = hardValue;
            this.veryHardValue = veryHardValue;
            this.extremeValue = extremeValue;
        }
    }
}
