using Archipelago.Structures;
using Reptile;

namespace Archipelago
{
    internal static class Requirements
    {
        public static RepValues HideoutStoryRep = new RepValues(20, 20);

        public static RepValues DownhillChallenge1Rep = new RepValues(30, 50);
        public static RepValues DownhillChallenge2Rep = new RepValues(38, 58);
        public static RepValues DownhillChallenge3Rep = new RepValues(45, 65);
        public static RepValues DownhillCrewWallRep = new RepValues(70, 90);
        public static ScoreValues DownhillCrewBattleScore = new ScoreValues(60000, 0, 0, 3500000);
        public static RepValues DownhillOldheadRep = new RepValues(100, 120);
        public static ScoreValues DownhillRaveScore = new ScoreValues(50000, 0, 0, 2000000);
        public static ScoreValues DownhillFrankScore = new ScoreValues(0, 0, 0, 0); // to do

        public static RepValues SquareEclipseRep = new RepValues(60, 180);
        public static RepValues SquareOldheadRep = new RepValues(120, 380);

        public static RepValues TowerChallenge1Rep = new RepValues(8, 188);
        public static RepValues TowerChallenge2Rep = new RepValues(20, 200);
        public static ScoreValues TowerChallenge2Score = new ScoreValues(25000, 0, 0, 400000);
        public static RepValues TowerChallenge3Rep = new RepValues(40, 220);
        public static RepValues TowerEscherRep = new RepValues(80, 260);
        public static RepValues TowerCrewWallRep = new RepValues(100, 280);
        public static ScoreValues TowerCrewBattleScore = new ScoreValues(150000, 0, 0, 7500000);
        public static RepValues TowerOldhead1Rep = new RepValues(70, 250);
        public static RepValues TowerOldhead2Rep = new RepValues(140, 320);
        public static ScoreValues TowerMeshScore = new ScoreValues(70000, 0, 0, 750000);
        public static ScoreValues TowerEclipseScore = new ScoreValues(0, 0, 0, 0); // to do

        public static RepValues MallChallenge1Rep = new RepValues(54, 434);
        public static RepValues MallChallenge2Rep = new RepValues(62, 442);
        public static RepValues MallChallenge3Rep = new RepValues(70, 450);
        public static RepValues MallChallenge4Rep = new RepValues(78, 458);
        public static ScoreValues MallChallenge4Score = new ScoreValues(100000, 0, 0, 2500000);
        public static RepValues MallCrewWallRep = new RepValues(111, 491);
        public static ScoreValues MallCrewBattle = new ScoreValues(300000, 0, 0, 12000000);
        public static RepValues MallOldhead1Rep = new RepValues(150, 530);
        public static RepValues MallOldhead2Rep = new RepValues(200, 580);
        public static ScoreValues MallDotEXEScore = new ScoreValues(0, 0, 0, 0); // to do

        public static RepValues PyramidGateRep = new RepValues(40, 620);
        public static RepValues PyramidChallenge1Rep = new RepValues(50, 630);
        public static ScoreValues PyramidChallenge1Score = new ScoreValues(70000, 0, 0, 600000);
        public static RepValues PyramidChallenge2Rep = new RepValues(60, 640);
        public static RepValues PyramidChallenge3Rep = new RepValues(70, 650);
        public static ScoreValues PyramidChallenge3Score = new ScoreValues(100000, 0, 0, 1000000);
        public static RepValues PyramidChallenge4Rep = new RepValues(80, 660);
        public static RepValues PyramidCrewRep = new RepValues(150, 730);
        public static ScoreValues PyramidCrewBattleScore = new ScoreValues(450000, 0, 0, 15000000);
        public static RepValues PyramidOldheadRep = new RepValues(200, 780);
        public static ScoreValues PyramidDevilTheoryScore = new ScoreValues(0, 0, 0, 0); // to do

        public static RepValues OsakaSmokeRep = new RepValues(70, 850);
        public static RepValues OsakaChallenge1Rep = new RepValues(84, 864);
        public static RepValues OsakaChallenge2Rep = new RepValues(100, 880);
        public static RepValues OsakaChallenge3Rep = new RepValues(140, 920);
        public static ScoreValues OsakaChallenge3Score = new ScoreValues(60000, 0, 0, 500000);
        public static RepValues OsakaCrewRep = new RepValues(180, 960);
        public static ScoreValues OsakaCrewBattleScore = new ScoreValues(600000, 0, 0, 18000000);
        public static RepValues OsakaOldheadRep = new RepValues(155, 935);
        public static ScoreValues OsakaFuturismScore = new ScoreValues(0, 0, 0, 0); // to do

        public static int? GetNPCNewRep(Stage stage, NPC npc)
        {
            switch (stage)
            {
                case Stage.downhill:
                    switch (npc.transform.parent.name)
                    {
                        case "WallrunsChallenge":
                            return DownhillChallenge1Rep.newValue;
                        case "ManualChallenge":
                            return DownhillChallenge2Rep.newValue;
                        case "CornersScoreChallenge":
                            return DownhillChallenge3Rep.newValue;
                        case "Crew_Wall":
                            return DownhillCrewWallRep.newValue;
                        case "OldHeadWall":
                            return DownhillOldheadRep.newValue;
                        default:
                            return null;
                    }
                case Stage.square:
                    switch (npc.transform.name)
                    {
                        case "NPC_EclipseCrew":
                            return SquareEclipseRep.newValue;
                        case "NPC_oldhead":
                            return SquareOldheadRep.newValue;
                        default:
                            return null;
                    }
                case Stage.tower:
                    switch (npc.transform.parent.name)
                    {
                        case "UpsideGrind_Challenge":
                            return TowerChallenge1Rep.newValue;
                        case "Manual_Challenge ":
                            return TowerChallenge2Rep.newValue;
                        case "Score_Challenge":
                            return TowerChallenge3Rep.newValue;
                        case "Chapter2":
                            switch (npc.transform.name)
                            {
                                case "CombatEncounter_SniperCaptain":
                                    return TowerEscherRep.newValue;
                                case "NPC_TowerGuard":
                                    return TowerCrewWallRep.newValue;
                                default:
                                    return null;
                            }
                        case "OldHeadWall":
                            switch (npc.requirement)
                            {
                                case 70:
                                    return TowerOldhead1Rep.newValue;
                                case 140:
                                    return TowerOldhead2Rep.newValue;
                                default:
                                    return null;
                            }
                        default:
                            return null;
                    }
                case Stage.Mall:
                    switch (npc.transform.parent.name)
                    {
                        case "Palms_Challenge":
                            return MallChallenge1Rep.newValue;
                        case "Slidepads_Challenge":
                            return MallChallenge2Rep.newValue;
                        case "Fish_Challenge ":
                            return MallChallenge3Rep.newValue;
                        case "Tricks_Challenge":
                            return MallChallenge4Rep.newValue;
                        case "Chapter3":
                            if (npc.transform.name == "NPC_Crew_GUARDING") return MallCrewWallRep.newValue;
                            else return null;
                        case "Oldhead1":
                            return MallOldhead1Rep.newValue;
                        case "Oldhead2":
                            return MallOldhead2Rep.newValue;
                        default:
                            return null;
                    }
                case Stage.pyramid:
                    switch (npc.transform.parent.name)
                    {
                        case "Chapter4":
                            if (npc.transform.name == "NPC_OnDoor") return PyramidGateRep.newValue;
                            else if (npc.transform.name == "NPC_Crew_CrewBattle_Starter") return PyramidCrewRep.newValue;
                            else return null;
                        case "Tricks_ScoreChallenge":
                            return PyramidChallenge1Rep.newValue;
                        case "RaceChallenge":
                            return PyramidChallenge2Rep.newValue;
                        case "Tricks2_ScoreChallenge":
                            return PyramidChallenge3Rep.newValue;
                        case "Manual_Challenge":
                            return PyramidChallenge4Rep.newValue;
                        case "OldHeadWall":
                            return PyramidOldheadRep.newValue;
                        default:
                            return null;
                    }
                case Stage.osaka:
                    switch (npc.transform.parent.name)
                    {
                        case "Chapter5":
                            if (npc.transform.name == "NPC_Crew_GateGuard") return OsakaSmokeRep.newValue;
                            else return null;
                        case "Race_Challenge":
                            return OsakaChallenge1Rep.newValue;
                        case "WallrunsChallenge":
                            return OsakaChallenge2Rep.newValue;
                        case "ScoreChallenge":
                            return OsakaChallenge3Rep.newValue;
                        case "OldHeadWall":
                        case "OldHeadWall2":
                            return OsakaOldheadRep.newValue;
                        case "CrewBattle":
                            return OsakaCrewRep.newValue;
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        public static RepValues GetLocalizedTextRepValues(string key)
        {
            switch (key)
            {
                case "Dialogue_hideout_54": // ln 159
                case "Dialogue_hideout_56": // ln 161
                    return HideoutStoryRep;
                case "Dialogue_downhill_8": // ln 269
                    return DownhillChallenge1Rep;
                case "Dialogue_downhill_21": // ln 287
                    return DownhillChallenge2Rep;
                case "Dialogue_downhill_33": // ln 304
                    return DownhillChallenge3Rep;
                case "Dialogue_downhill_45": // ln 321
                case "Dialogue_downhill_49": // ln 327
                    return DownhillCrewWallRep;
                case "Dialogue_downhill_254": // ln 559
                    return DownhillOldheadRep;
                case "Dialogue_tower_82": // ln 858
                    return TowerCrewWallRep;
                case "Dialogue_tower_205": // ln 1001
                    return TowerOldhead1Rep;
                case "Dialogue_tower_245": // ln 1051
                case "Dialogue_tower_246": // ln 1053
                    return TowerOldhead2Rep;
                case "Dialogue_square_51": // ln 1146
                    return SquareOldheadRep;
                case "Dialogue_Mall_32": // ln 1249
                    return MallChallenge1Rep;
                case "Dialogue_Mall_61": // ln 1289
                    return MallChallenge3Rep;
                case "Dialogue_Mall_96": // ln 1334
                case "Dialogue_Mall_103": // ln 1343
                    return MallCrewWallRep;
                case "Dialogue_Mall_205": // ln 1462
                    return MallOldhead2Rep;
                case "Dialogue_Mall_216": // ln 1475
                    return MallOldhead1Rep;
                case "Dialogue_pyramid_5": // ln 1708
                case "Dialogue_pyramid_7": // ln 1711
                    return PyramidGateRep;
                case "Dialogue_pyramid_71": // ln 1823
                case "Dialogue_pyramid_74": // ln 1826
                case "Dialogue_pyramid_75": // ln 1828
                    return PyramidCrewRep;
                case "Dialogue_pyramid_209": // ln 1972
                    return PyramidOldheadRep;
                case "Dialogue_osaka_57": // ln 2184
                    return OsakaCrewRep;
                case "Dialogue_osaka_263": // ln 2580
                case "Dialogue_osaka_274": // ln 2593
                    return OsakaOldheadRep;
                case "NOTIFICATION_DOWNHILL_REPFRANKSHIDEOUT": // ln 13
                    return DownhillCrewWallRep;
                case "NOTIFICATION_PYRAMID_REPCREWBATTLE": // ln 27
                    return PyramidCrewRep;
                case "NOTIFICATION_OSAKA_REPCREWBATTLE": // ln 31
                    return OsakaCrewRep;
                default:
                    return null;
            }
        }
    }
}
