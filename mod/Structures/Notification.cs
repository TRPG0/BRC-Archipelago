using Reptile;

namespace Archipelago.BRC.Structures
{
    public class Notification
    {
        public string app;
        public string message;
        public AUnlockable unlockable;

        public Notification(string app, string message, AUnlockable unlockable)
        {
            this.app = app;
            this.message = message;
            this.unlockable = unlockable;
        }
    }
}
