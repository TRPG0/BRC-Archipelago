using Archipelago.Structures;
using Reptile;

namespace Archipelago
{
    internal static class RepRequirements
    {
        public static RepValues HideoutStory = new RepValues() { oldValue = 20, newValue = 20 };

        public static RepValues DownhillChallenge1 = new RepValues() { oldValue = 30, newValue = 50 };
        public static RepValues DownhillChallenge2 = new RepValues() { oldValue = 38, newValue = 58 };
        public static RepValues DownhillChallenge3 = new RepValues() { oldValue = 45, newValue = 65 };
        public static RepValues DownhillCrewWall = new RepValues() { oldValue = 70, newValue = 90 };
        public static RepValues DownhillOldhead = new RepValues() { oldValue = 100, newValue = 120 };

        public static RepValues SquareEclipse = new RepValues() { oldValue = 60, newValue = 180 };
        public static RepValues SquareOldhead = new RepValues() { oldValue = 120, newValue = 380 };

        public static RepValues TowerChallenge1 = new RepValues() { oldValue = 8, newValue = 188 };
        public static RepValues TowerChallenge2 = new RepValues() { oldValue = 20, newValue = 200 };
        public static RepValues TowerChallenge3 = new RepValues() { oldValue = 40, newValue = 220 };
        public static RepValues TowerEscher = new RepValues() { oldValue = 80, newValue = 260 };
        public static RepValues TowerCrewWall = new RepValues() { oldValue = 100, newValue = 280 };
        public static RepValues TowerOldhead1 = new RepValues() { oldValue = 70, newValue = 250 };
        public static RepValues TowerOldhead2 = new RepValues() { oldValue = 140, newValue = 320 };

        public static RepValues MallChallenge1 = new RepValues() { oldValue = 54, newValue = 434 };
        public static RepValues MallChallenge2 = new RepValues() { oldValue = 62, newValue = 442 };
        public static RepValues MallChallenge3 = new RepValues() { oldValue = 70, newValue = 450 };
        public static RepValues MallChallenge4 = new RepValues() { oldValue = 78, newValue = 458 };
        public static RepValues MallCrewWall = new RepValues() { oldValue = 111, newValue = 491 };
        public static RepValues MallOldhead1 = new RepValues() { oldValue = 150, newValue = 530 };
        public static RepValues MallOldhead2 = new RepValues() { oldValue = 200, newValue = 580 };

        public static RepValues PyramidGate = new RepValues() { oldValue = 40, newValue = 620 };
        public static RepValues PyramidChallenge1 = new RepValues() { oldValue = 50, newValue = 630 };
        public static RepValues PyramidChallenge2 = new RepValues() { oldValue = 60, newValue = 640 };
        public static RepValues PyramidChallenge3 = new RepValues() { oldValue = 70, newValue = 650 };
        public static RepValues PyramidChallenge4 = new RepValues() { oldValue = 80, newValue = 660 };
        public static RepValues PyramidCrew = new RepValues() { oldValue = 150, newValue = 730 };
        public static RepValues PyramidOldhead = new RepValues() { oldValue = 200, newValue = 780 };

        public static RepValues OsakaSmoke = new RepValues() { oldValue = 70, newValue = 850 };
        public static RepValues OsakaChallenge1 = new RepValues() { oldValue = 84, newValue = 864 };
        public static RepValues OsakaChallenge2 = new RepValues() { oldValue = 100, newValue = 880 };
        public static RepValues OsakaChallenge3 = new RepValues() { oldValue = 140, newValue = 920 };
        public static RepValues OsakaCrew = new RepValues() { oldValue = 180, newValue = 960 };
        public static RepValues OsakaOldhead = new RepValues() { oldValue = 155, newValue = 935 };

        public static int? GetNPCNewRep(Stage stage, NPC npc)
        {
            switch (stage)
            {
                case Stage.downhill:
                    switch (npc.transform.parent.name)
                    {
                        case "WallrunsChallenge":
                            return DownhillChallenge1.newValue;
                        case "ManualChallenge":
                            return DownhillChallenge2.newValue;
                        case "CornersScoreChallenge":
                            return DownhillChallenge3.newValue;
                        case "Crew_Wall":
                            return DownhillCrewWall.newValue;
                        case "OldHeadWall":
                            return DownhillOldhead.newValue;
                        default:
                            return null;
                    }
                case Stage.square:
                    switch (npc.transform.name)
                    {
                        case "NPC_EclipseCrew":
                            return SquareEclipse.newValue;
                        case "NPC_oldhead":
                            return SquareOldhead.newValue;
                        default:
                            return null;
                    }
                case Stage.tower:
                    switch (npc.transform.parent.name)
                    {
                        case "UpsideGrind_Challenge":
                            return TowerChallenge1.newValue;
                        case "Manual_Challenge ":
                            return TowerChallenge2.newValue;
                        case "Score_Challenge":
                            return TowerChallenge3.newValue;
                        case "Chapter2":
                            switch (npc.transform.name)
                            {
                                case "CombatEncounter_SniperCaptain":
                                    return TowerEscher.newValue;
                                case "NPC_TowerGuard":
                                    return TowerCrewWall.newValue;
                                default:
                                    return null;
                            }
                        case "OldHeadWall":
                            switch (npc.requirement)
                            {
                                case 70:
                                    return TowerOldhead1.newValue;
                                case 140:
                                    return TowerOldhead2.newValue;
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
                            return MallChallenge1.newValue;
                        case "Slidepads_Challenge":
                            return MallChallenge2.newValue;
                        case "Fish_Challenge ":
                            return MallChallenge3.newValue;
                        case "Tricks_Challenge":
                            return MallChallenge4.newValue;
                        case "Chapter3":
                            if (npc.transform.name == "NPC_Crew_GUARDING") return MallCrewWall.newValue;
                            else return null;
                        case "Oldhead1":
                            return MallOldhead1.newValue;
                        case "Oldhead2":
                            return MallOldhead2.newValue;
                        default:
                            return null;
                    }
                case Stage.pyramid:
                    switch (npc.transform.parent.name)
                    {
                        case "Chapter4":
                            if (npc.transform.name == "NPC_OnDoor") return PyramidGate.newValue;
                            else if (npc.transform.name == "NPC_Crew_CrewBattle_Starter") return PyramidCrew.newValue;
                            else return null;
                        case "Tricks_ScoreChallenge":
                            return PyramidChallenge1.newValue;
                        case "RaceChallenge":
                            return PyramidChallenge2.newValue;
                        case "Tricks2_ScoreChallenge":
                            return PyramidChallenge3.newValue;
                        case "Manual_Challenge":
                            return PyramidChallenge4.newValue;
                        case "OldHeadWall":
                            return PyramidOldhead.newValue;
                        default:
                            return null;
                    }
                case Stage.osaka:
                    switch (npc.transform.parent.name)
                    {
                        case "Chapter5":
                            if (npc.transform.name == "NPC_Crew_GateGuard") return OsakaSmoke.newValue;
                            else return null;
                        case "Race_Challenge":
                            return OsakaChallenge1.newValue;
                        case "WallrunsChallenge":
                            return OsakaChallenge2.newValue;
                        case "ScoreChallenge":
                            return OsakaChallenge3.newValue;
                        case "OldHeadWall":
                        case "OldHeadWall2":
                            return OsakaOldhead.newValue;
                        case "CrewBattle":
                            return OsakaCrew.newValue;
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
                    return HideoutStory;
                case "Dialogue_downhill_8": // ln 269
                    return DownhillChallenge1;
                case "Dialogue_downhill_21": // ln 287
                    return DownhillChallenge2;
                case "Dialogue_downhill_33": // ln 304
                    return DownhillChallenge3;
                case "Dialogue_downhill_45": // ln 321
                case "Dialogue_downhill_49": // ln 327
                    return DownhillCrewWall;
                case "Dialogue_downhill_254": // ln 559
                    return DownhillOldhead;
                case "Dialogue_tower_82": // ln 858
                    return TowerCrewWall;
                case "Dialogue_tower_205": // ln 1001
                    return TowerOldhead1;
                case "Dialogue_tower_245": // ln 1051
                case "Dialogue_tower_246": // ln 1053
                    return TowerOldhead2;
                case "Dialogue_square_51": // ln 1146
                    return SquareOldhead;
                case "Dialogue_Mall_32": // ln 1249
                    return MallChallenge1;
                case "Dialogue_Mall_61": // ln 1289
                    return MallChallenge3;
                case "Dialogue_Mall_96": // ln 1334
                case "Dialogue_Mall_103": // ln 1343
                    return MallCrewWall;
                case "Dialogue_Mall_205": // ln 1462
                    return MallOldhead2;
                case "Dialogue_Mall_216": // ln 1475
                    return MallOldhead1;
                case "Dialogue_pyramid_5": // ln 1708
                case "Dialogue_pyramid_7": // ln 1711
                    return PyramidGate;
                case "Dialogue_pyramid_71": // ln 1823
                case "Dialogue_pyramid_74": // ln 1826
                case "Dialogue_pyramid_75": // ln 1828
                    return PyramidCrew;
                case "Dialogue_pyramid_209": // ln 1972
                    return PyramidOldhead;
                case "Dialogue_osaka_57": // ln 2184
                    return OsakaCrew;
                case "Dialogue_osaka_263": // ln 2580
                case "Dialogue_osaka_274": // ln 2593
                    return OsakaOldhead;
                case "NOTIFICATION_DOWNHILL_REPFRANKSHIDEOUT": // ln 13
                    return DownhillCrewWall;
                case "NOTIFICATION_PYRAMID_REPCREWBATTLE": // ln 27
                    return PyramidCrew;
                case "NOTIFICATION_OSAKA_REPCREWBATTLE": // ln 31
                    return OsakaCrew;
                default:
                    return null;
            }
        }
    }
}
