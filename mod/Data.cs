using System.Collections.Generic;
using Reptile;

namespace Archipelago
{
    public class Data
    {
        public long index;
        public string host_name;
        public string slot_name;
        public string password;
        public HashSet<string> @checked = new HashSet<string>();

        public HashSet<string> to_lock = new HashSet<string>()
        {
            "OVERWHELMME",
            "QUICK BING",
            "WHOLE SIXER",
            "Graffo Le Fou",
            "WILD STRUXXA",
            "Bombing by FireMan"
        };

        public bool hasM = false;

        public int fakeRep = 0;
        public int sprayCount = 0;

        public bool skateboardUnlocked = true;
        public bool inlineUnlocked = false;
        public bool bmxUnlocked = false;

        public Characters firstCharacter = Characters.NONE;
        public bool dummyUnlocked = false;

        public bool death_link = false;
    }
}
