using System;
using System.Collections.Generic;
using System.Text;

namespace PixelWorldsServer2.Database
{
    public struct ShopResult
    {
        public int price;
        public List<int> items;
    }
    public class Shop
    {
        public static Dictionary<string, ShopResult> offers = new Dictionary<string, ShopResult>();

        public static void AddShopOffer(string name, int price, params int[] items)
        {
            ShopResult sr = new ShopResult();
            sr.items = new List<int>(items);
            sr.price = price;

            offers[name] = sr;
        }

        public static bool ContainsItem(int item)
        {
            foreach (var res in offers.Values)
            {
                if (res.items.Contains(item))
                    return true;
            }

            return false;
        }

        public static void Init()
        {
            AddShopOffer("WorldLock", 3500, 413);
            AddShopOffer("PlatinumLock", 3500 * 100, 796);
            AddShopOffer("PetFoodDogPremium", 1250, 3856);
            AddShopOffer("SmallLock", 100, 410);
            AddShopOffer("MediumLock", 500, 411);
            AddShopOffer("LargeLock", 1000, 412);
            AddShopOffer("BattleLock", 3000, 1132);
            AddShopOffer("BattleWorldLock", 7500, 3060);
            AddShopOffer("DarkWorldLock", 28000, 882);
            AddShopOffer("FrostWings", 100000, 2608);
            AddShopOffer("PixieWings", 7500, 586);
            AddShopOffer("GreenContactLenses", 7500, 906);
            AddShopOffer("BlueDragonWings", 75000, 880);
            AddShopOffer("SodaJetpack", 400000, 881);
            AddShopOffer("BlueContactLenses", 7500, 905);
            AddShopOffer("BrownContactLenses", 7500, 908);
            AddShopOffer("SilverContactLenses", 7500, 909);
            AddShopOffer("TurquoiseContactLenses", 22000, 913);
            AddShopOffer("GoldContactLenses", 28000, 907);
            AddShopOffer("PurpleContactLenses", 30000, 910);
            AddShopOffer("WhiteContactLenses", 125000, 911);
            AddShopOffer("RedContactLenses", 150000, 609);
            AddShopOffer("AlienLenses", 300000, 3088);
            AddShopOffer("GoblinRing", 60000, 935);
            AddShopOffer("FrostRing", 150000, 934);
            AddShopOffer("DemonRing", 150000, 1293);
            AddShopOffer("LemonRing", 200000, 3085);
            AddShopOffer("OceanRing", 225000, 3086);
            AddShopOffer("RoseRing", 250000, 3087);
            AddShopOffer("FishingRodBambooBasic", 500, 2406);
            AddShopOffer("FishingRodFiberglassBasic", 2500, 2410);
            AddShopOffer("FishingRodCarbonFiberBasic", 15000, 2414);
            AddShopOffer("FishingRodTitaniumBasic", 35000, 2418);
            AddShopOffer("FishingRodUpgradeStation", 10000, 2506);
            AddShopOffer("FishingScoreBoard", 15000, 2535);
            AddShopOffer("FishingRecycler", 20000, 2504);
            AddShopOffer("SupportHoodie", 10000, 879);
            AddShopOffer("PinkSupportHoodie", 10000, 1023);
            AddShopOffer("WingsDemon", 12000, 215);
            AddShopOffer("Fertilizer", 150, 1070);
            AddShopOffer("FertilizerLarge", 300, 1499);
            AddShopOffer("Snowman", 55000, 1458);
            AddShopOffer("Penguin", 70000, 1463);
            AddShopOffer("Bunny", 35000, 1095);
            AddShopOffer("Crow", 50000, 1093);
            AddShopOffer("Mini-bot", 65000, 1100);
            AddShopOffer("Gremlin", 75000, 1086);
            AddShopOffer("FAMEvolverator", 35000, 1126);
            AddShopOffer("FAMFoodMachine", 50000, 1125);
            AddShopOffer("VirtualPetDog", 10000, 3822);
            AddShopOffer("VirtualPetCat", 10000, 3823);
            AddShopOffer("PetFoodDogBasic", 500, 3855);
            AddShopOffer("PetFoodCatBasic", 500, 3857);
            AddShopOffer("PetFoodCatPremium", 1250, 3858);
            AddShopOffer("PetFoodSlimeBasic", 500, 3859);
            AddShopOffer("PetFoodSlimePremium", 1250, 3860);
            AddShopOffer("PetMedicineBasic", 500, 3861);
            AddShopOffer("OrbLightingLesserDark", 75000, 4141);
            AddShopOffer("OrbLightingDark", 75000, 3922);
            AddShopOffer("OrbLightingNone", 2000, 3921);
            AddShopOffer("OrbWeatherNone", 1000, 3370);
            AddShopOffer("OrbWeatherLightRain", 10000, 3444);
            AddShopOffer("OrbWeatherSandStorm", 17500, 3443);
            AddShopOffer("WinterOrb", 10000, 521);
            AddShopOffer("ForestOrb", 2000, 520);
            AddShopOffer("StarOrb", 5000, 524);
            AddShopOffer("SandOrb", 9000, 519);
            AddShopOffer("NightOrb", 12000, 522);
            AddShopOffer("CityOrb", 100, 1758);
            AddShopOffer("WeaponWiringTool", 7500, 3097);
            AddShopOffer("WiringTriggerLever", 250, 3100);
            AddShopOffer("WiringTriggerSwitch", 250, 3098);
            AddShopOffer("WiringTriggerButton", 350, 3099);
            AddShopOffer("WiringTriggerPressurePad", 500, 3101);
            AddShopOffer("WiringTriggerProximitySensor", 500, 3102);
            AddShopOffer("OnOffLight", 100, 3111);
            AddShopOffer("DisappearingBlock", 250, 3112);
            AddShopOffer("WiringLogicGateAND", 250, 3103);
            AddShopOffer("WiringLogicGateNAND", 250, 3104);
            AddShopOffer("WiringLogicGateOR", 250, 3105);
            AddShopOffer("WiringLogicGateNOR", 250, 3106);
            AddShopOffer("WiringLogicGateXOR", 250, 3107);
            AddShopOffer("WiringLogicGateXNOR", 250, 3108);
            AddShopOffer("WiringLogicGateNOT", 250, 3109);
            AddShopOffer("WiringLogicGateSIGNALDIVIDER", 150, 3146);
            AddShopOffer("WiringLogicGateTOGGLE", 500, 3167);
            AddShopOffer("WiringLogicGateDELAYTIMER", 500, 3143);
            AddShopOffer("WiringLogicGateSIGNALHOLDER", 500, 3144);
            AddShopOffer("WiringLogicGateTIMER", 500, 3145);
            AddShopOffer("WiringLogicGateRANDOMIZER", 500, 3183);
            AddShopOffer("ConsumableRedScroll", 150, 1402);
            AddShopOffer("ConsumableRedScroll10", 1000, 1402, 1402, 1402, 1402, 1402, 1402, 1402, 1402, 1402, 1402);
            AddShopOffer("RedPortal", 9000, 1799);
            AddShopOffer("JetRaceGroupPortal", 45000, 4373);
            AddShopOffer("ScreenshotForbidden", 100, 3442);
            AddShopOffer("ConsumableCameraWorld", 100, 1521);
            AddShopOffer("PrizeBox", 350, 966);
            AddShopOffer("AdTV", 500, 3052);
            AddShopOffer("Recall", 1000, 2343);
            AddShopOffer("RatingBoard", 1200, 293);
            AddShopOffer("DeathCounter", 2000, 970);
            AddShopOffer("EntrancePortal", 3750, 1078);
            AddShopOffer("MagicCauldron", 5750, 294);
            AddShopOffer("RuleBot", 10000, 1358);
            AddShopOffer("RuleBotPotion", 10000, 2332);
            AddShopOffer("RuleBotMount", 125000, 4367);
            AddShopOffer("BestSetPhotoBooth", 15000, 4491);
            AddShopOffer("SafeBox", 17500, 3576);
            AddShopOffer("Replicator", 20000, 847);
            AddShopOffer("ColorOMat", 25000, 3437);
            AddShopOffer("GravityModifier", 150000, 2008);
            AddShopOffer("BlueprintJetPackSnow", 425000, 3525);
            AddShopOffer("BlueprintNecklaceFrost", 80000, 1447);
            AddShopOffer("BlueprintOrbSpaceBackground", 50000, 856);
            AddShopOffer("BlueprintNecklaceGlimmer", 60000, 863);
            AddShopOffer("BlueprintMaskTiki", 100000, 861);
            AddShopOffer("BlueprintWeaponSwordLaserGreen", 350000, 853);
            AddShopOffer("BlueprintWeaponSwordLaserRed", 350000, 854);
            AddShopOffer("BlueprintWeaponSwordLaserBlue", 100, 855);
            AddShopOffer("BlueprintShirtArmorKnight", 225000, 3342);
            AddShopOffer("BlueprintPantsArmorKnight", 225000, 3343);
            AddShopOffer("BlueprintHatHelmetArmorKnight", 225000, 3344);
            AddShopOffer("BlueprintWeaponSwordKnight", 350000, 3345);
            AddShopOffer("BlueprintCapeDark", 475000, 857);
            AddShopOffer("BlueprintWeaponSwordLaserClaymore", 500000, 3091);
            AddShopOffer("BlueprintJetPackPlasma", 500000, 862);
            AddShopOffer("BlueprintWeaponSwordFlaming", 725000, 864);
            AddShopOffer("BlueprintWingsValkyria", 750000, 1289);
            AddShopOffer("BlueprintWingsMechanicalGolden", 950000, 3089);
            AddShopOffer("BlueprintJetPackLongJumpAncientGolem", 500000, 4779);
            AddShopOffer("BlueprintWingsBackgoyle", 750000, 4777);
            AddShopOffer("BlueprintJetPackLongJumpExplosive", 500000, 4775);
            AddShopOffer("BlueprintWingsIonThrusters", 500000, 4760);
            AddShopOffer("BlueprintBackBackpackWanderer", 335000, 4746);
            AddShopOffer("KiddieRide", 10000, 1129);
            AddShopOffer("DoorClan", 1000, 3559);
            AddShopOffer("ClanQuestBot", 7500, 3500);
            AddShopOffer("ClanTotem", 9000, 3466);
            AddShopOffer("DoorFactionDark", 2000, 3602);
            AddShopOffer("DoorFactionLight", 2000, 3603);
            AddShopOffer("CheckPointFactionDark", 3500, 3598);
            AddShopOffer("CheckPointFactionLight", 3500, 3599);
            AddShopOffer("PortalFactionDark", 4000, 3600);
            AddShopOffer("PortalFactionLight", 100, 3601);
            AddShopOffer("BattleScoreBoardFaction", 8000, 3597);
            AddShopOffer("LockBattleFaction", 7500, 3596);
            AddShopOffer("LockWorldBattleFaction", 9000, 3606);


        }
    }
}
