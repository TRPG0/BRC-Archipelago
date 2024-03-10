namespace Archipelago.BRC.Structures
{
    public class RepValues : Values
    {
        public int newValue;

        public RepValues(int oldValue, int newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
