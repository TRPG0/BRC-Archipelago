using System.Collections.Generic;
using Archipelago.Structures;
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

        public bool skipIntro = true;
        public bool skipDreams = false;
        public TotalRep totalRep = TotalRep.Normal;
        public ScoreDifficulty scoreDifficulty = ScoreDifficulty.Normal;

        public HashSet<string> to_lock = new HashSet<string>()
        {
            "OVERWHELMME",
            "QUICK BING",
            "WHOLE SIXER",
            "Graffo Le Fou",
            "WILD STRUXXA",
            "Bombing by FireMan"
        };

        public bool limitedGraffiti = false;
        public Dictionary<string, int> grafUses = new Dictionary<string, int>();

        public int fakeRep = 0;
        public int sprayCount = 0;

        public MoveStyle startingMovestyle = MoveStyle.SKATEBOARD;
        public bool skateboardUnlocked = true;
        public bool inlineUnlocked = false;
        public bool bmxUnlocked = false;

        public Characters firstCharacter = Characters.NONE;
        public bool dummyUnlocked = false;

        public int damageMultiplier = 1;
        public bool deathLink = false;
    }
}
