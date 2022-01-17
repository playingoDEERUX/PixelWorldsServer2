/* Creation Date: 26.12.2019 */
/* Author: @playingo */

using Kernys.Bson;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PixelWorldsServer2.DataManagement
{
    class MsgLabels
    {
        public const string MessageID = "ID";
        public const string MessageCount = "mc";
        public const string MessageIndex = "m";

        public const string UserID = "U"; // also used for gpd
        public const string OperatingSystem = "OS";
        public const string VersionNumberKey = "VN";
        public const string Category = "cgy";
        public const string Count = "Ct";
        public const string CognitoId = "CoID";
        public const string CognitoToken = "Tk";
        public const string SequencingInterval = "SSlp";
        public const string TimeStamp = "STime";
        public const string JoinResult = "JR";
        public const string ChatMessageBinary = "CmB";
        public const string Time = "T";
        public const string DestroyBlockBlockType = "DBBT";
        public const string StatusIcon = "SIc";

        public static BSONObject pingBson = new BSONObject("p");

        public enum JR 
        {
            SUCCESS,
            UNAVAILABLE = 2,
            MAINTENANCE,
            INVALID_NAME
        }
        public struct PlayerData // pD
        {
            public const string ByteCoinAmount = "bcs";
            public const string GemsAmount = "gems";
            public const string Username = "UN";
            public const string RealUsername = "rUN";
            public const string InventorySlots = "slots";
            public const string PlayerOPStatus = "playerAdminStatusKey";
            public const string inventoryBinary = "inv";
            public const string inventoryData = "invData";
            public const string CameraZoomLevel = "cameraZoomLevel";
            public const string FamiliarName = "famName"; // active pet name??
            public const string IsFamiliarMaxLevel = "isFamMaxLvl"; // boolean??
            public const string FaceAnimation = "faceAnim"; // active face animation
            public const string Skin = "skin"; // skin color ??
            public const string Gender = "gender";
            public const string NormalClaimTime = "normalClaimTime";
            public const string VIPClaimTime = "vipClaimTime";
            public const string HasClaimedAdditional = "hasClaimedAdditional"; // idk what its for, boolean??
            public const string Statistics = "statistics"; // List??
            public const string AccountAge = "accountAge";
            public const string CountryCode = "countryCode";
            public const string VIPEndTime = "VIPendTime";
            public const string NameChangeCounter = "nameChangeCounter";
            public const string ExperienceAmount = "experienceAmount";
            public const string XPAmount = "xpAmount";
            public const string NextDailyBonusGiveAway = "nextdailyBonusGiveAwayKey";
            public const string NextNormalDailyBonusClaim = "nextNormalDailyBonusClaimKey";
            public const string NextVIPDailyBonusClaim = "nextVIPDailyBonusClaimKey";
            public const string NextDailyAdsRefreshTime = "nextDailyAdsRefreshTimeKey";
            public const string NextDailyPvpRewardsRefreshTime = "nextDailyPvpRewardsRefreshTimeKey";
            //FPCKey
            public const string ShowLocation = "showLocation"; // boolean??
            public const string ShowOnlineStatus = "showOnlineStatus"; // boolean??
            //gV
            public const string PunchEffects = "pEffs"; // active punchEffects??
            public const string PlayerAmounts = "playerAmounts"; //?
            public const string InstructionStates = "instructionStates";
            public const string AchievementCurrentValues = "achievementCurrentValues";
            public const string AchievementsCompletedStates = "achievementsCompletedStates";
            public const string AchievementRewardsClaimed = "achievementRewardsClaimed";
            public const string QuestListCount = "questListCountKey";
            public const string QuestCurrentID = "questCurrentIDKey";
            public const string QuestCurrentPhase = "questCurrentPhaseKey";
            public const string FaceExpressionListID = "faceExpressionListIDKey";
            public const string BoughtExpressionsList = "boughtExpressionsListKey";
            public const string TutorialQuestCompleteState = "tutorialQuestCompleteState";
            public const string AlreadyBoughtOneTimeItems = "alreadyBoughtOneTimeItemsKey";
            public const string DailyQuestNextAvailableList = "dailyQuestNextAvailListKey";
            public const string PreviousThreeDailyQuestIds = "previousThreeDailyQuestIdsKey";
            public const string LastTutorialQuest = "lastTutorialQuest";
            public const string Tutorial1 = "Tutorial1";
            public const string Tutorial1CurrentStep = "Tutorial1currentStep";
            public const string Tutorial1TrackQuestStepProgress = "Tutorial1trackQuestStepProgress";
            public const string LimitedOffersKey = "limitedOffersKey";
        }

        // ChatMessage
        public struct ChatMessage
        {
           
            public const string Nickname = "nick";
            public const string UserID = "userID";
            public const string Channel = "channel";
            public const string Message = "message";
            public const string ChatTime = "time";
        }



        public struct Ident // Stands for: Identification. Every string value that is used for BsonObject["ID"] = {...} goes here
        {
            public const string Ping = "p";
            public const string MovePlayer = "mP";
            public const string VersionCheck = "VChk";
            public const string GetPlayerData = "GPd";
            public const string GetWorld = "Gw";
            public const string GetWorldCompressed = "GWC";
            public const string OtherOwnerIP = "OoIP"; // switch servers
            public const string SyncTime = "ST";
            public const string TryToJoinWorld = "TTjW";
            public const string WearableUsed = "WeOwC";
            public const string WearableRemoved = "WeOwU";
            //public const string RemoveBlock = "RB";
            public const string SetBlock = "SB";
            public const string HitBlock = "HB";
            public const string SetBlockWater = "SBW";
            public const string HitBlockWater = "HBW";
            public const string DestroyBlock = "DB";
            public const string SetBackgroundBlock = "SBB";
            public const string HitBackgroundBlock = "HBB";
            public const string DestroyBackgroundBlock = "DBB";
            public const string WorldItemUpdate = "WIU";

            public const string BroadcastGlobalMessage = "BGM";
            public const string LeaveWorld = "LW";
            public const string AnotherPlayer = "AnP"; // used to send a spawn signal to other player(s).
           
        }
    }
}
