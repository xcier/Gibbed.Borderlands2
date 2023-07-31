using Gibbed.Borderlands2.GameInfo;
using Gibbed.Borderlands2.ProtoBufFormats.WillowTwoSave;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ProtoBuf;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gibbed.Borderlands2.FileFormats
{

    public class SaveFileJSON
    {
        public class SaveFileJSONFlaotConverter : JsonConverter<float>
        {
            public override float ReadJson(JsonReader reader, Type objectType,
                float existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer,
                float value, JsonSerializer serializer)
            {

                writer.WriteRawValue(value.ToString("g8"));
            }
        }
        public SaveFile ToSaveFile()
        {
            var savefile = new SaveFile();
            savefile.Platform = GameInfo.Platform.PC;
            savefile.PlayerStats = new PlayerStats();
            var save = new WillowTwoPlayerSaveGame
            {
                // Not found
                AwesomeSkillDisabled = false,
                // Not sure
                PlayerHasPlayedInPlaythroughThree = null,
                // Not found
                ChosenVehicleCustomizations = new List<ChosenVehicleCustomization>(),
                // Empty
                DLCExpansionData = new List<DLCExpansionData>(),
                // Empty or not found
                ItemData = new List<ItemData>(),
                // Empty
                ItemMementos = new List<ItemMemento>(),
                // Not found
                LastOverpowerChoice = null,
                // Not found
                LockoutList = new List<LockoutData>(),
                // Not found
                WeaponMementos = new List<WeaponMemento>(),
                // Not found
                NumOverpowerLevelsUnlocked = null,
                // Not found
                QueuedTrainingMessages = new List<string>(),
                // Empty
                WeaponData = new List<WeaponData>(),
                // Not found
                Unknown10 = new List<int>(),
                // Not found
                Unknown28 = new List<string>(),
                // Empty
                Unknown47 = new List<GoldenKeys>(),
                // Not found
                Unknown9 = new List<int>(),

                VehicleSteeringMode = field58,
                UsedMarketingCodes = field23.ToList(),
                MarketingCodesNeedingNotification = field24.ToList(),
                Unknown22 = field22,
                PlotMissionNumber = field21,
                StatsData = Convert.FromBase64String(field15),
                VisitedTeleporters = field16.ToList(),
                LastVisitedTeleporter = field17,
                RegionGameStages = saved_regions.Select(r => (RegionGameStageData)r).ToList(),
                WorldDiscoveryList = world_discovery_list.Select(d => (WorldDiscoveryData)d).ToList(),
                TotalPlayTime = total_play_time,
                SpecialistSkillPoints = special_skill_points,
                PlaythroughsCompleted = playthroughs_completed,
                SaveGuid = save_guid,
                SkillData = skill_list.Select(s => (SkillData)s).ToList(),
                ResourceData = resource_data.Select(r => (ResourceData)r).ToList(),
                UIPreferences = preferences_data,
                MissionPlaythroughs = playthrough_data.Select(p => (MissionPlaythroughData)p).ToList(),
                LastPlaythroughNumber = last_play_through_index,
                LevelChallengeUnlocks = level_challenge_unlocks.ToList(),
                PackedItemData = packed_item_data.Select(i => (PackedItemData)i).ToList(),
                OneOffLevelChallengeCompletion = one_off_level_challenge_data.Select(o => (OneOffLevelChallengeData)o).ToList(),
                NumChallengePrestiges= num_challenge_prestiges,
                NumGoldenKeysNotified = num_golden_keys_notified,
                PackedWeaponData = packed_weapon_data.Select(p => (PackedWeaponData)p).ToList(),
                SaveGameId = save_game_id,
                ReceivedDefaultWeapon = received_default_weapon,
                ShowNewPlaythroughNotification= show_new_playthrough_notification,
                InventorySlotData = inventory_data,
                LastSavedDate = last_saved_date,
                MaxBankSlots = max_bank_slots,
                CurrencyOnHand = currency.ToList(),
                FullyExploredAreas = completey_explored_areas.ToList(),
                ActiveMissionNumber = active_mission_number,
                AppliedCustomizations = applied_customizations.ToList(),
                BankSlots = bank_slots.Select(s => (BankSlot)s).ToList(),
                BlackMarketUpgrades = black_market_upgrades.ToList(),
                ChallengeList = challenge_data.Select(c => (ChallengeData)c).ToList(),
                ExpLevel = player_level,
                IsDLCPlayerClass = is_dlc_player_class,
                IsBadassModeSaveGame = is_badass_mode,
                GeneralSkillPoints  = general_skill_points,
                PlayerClass = player_class,
                DLCPlayerClassPackageId = dlc_player_class_id,
                ExpPoints = experience_points,

            };
            savefile.SaveGame = save;

            return savefile;

        }
        
        
        public void LoadSavedGame(WillowTwoPlayerSaveGame saveGame)
        {
            field15 = Convert.ToBase64String(saveGame.StatsData);
            field16 = saveGame.VisitedTeleporters.ToArray();
            field17 = saveGame.LastVisitedTeleporter;
            field21 = saveGame.PlotMissionNumber;
            field22 = saveGame.Unknown22;
            field23 = saveGame.UsedMarketingCodes.ToArray();
            field24 = saveGame.MarketingCodesNeedingNotification.ToArray();
            field58 = saveGame.VehicleSteeringMode.GetValueOrDefault();
            saved_regions = saveGame.RegionGameStages.Select(r => Saved_Regions.From(r)).ToArray();
            world_discovery_list = saveGame.WorldDiscoveryList.Select(d => World_Discovery_List.From(d)).ToArray();
            total_play_time = saveGame.TotalPlayTime;
            special_skill_points = saveGame.SpecialistSkillPoints;
            playthroughs_completed = saveGame.PlaythroughsCompleted;
            save_guid = Save_Guid.From(saveGame.SaveGuid);
            skill_list = saveGame.SkillData.Select(s => Skill_List.From(s)).ToArray();
            resource_data = saveGame.ResourceData.Select(r => Resource_Data.From(r)).ToArray();
            preferences_data = Preferences_Data.From(saveGame.UIPreferences);
            playthrough_data = saveGame.MissionPlaythroughs.Select(p => Playthrough_Data.From(p)).ToArray();
            last_play_through_index = saveGame.LastPlaythroughNumber;
            level_challenge_unlocks = saveGame.LevelChallengeUnlocks.ToArray();
            packed_item_data = saveGame.PackedItemData.Select(i => Packed_Item_Data.From(i)).ToArray();
            one_off_level_challenge_data = saveGame.OneOffLevelChallengeCompletion.Select(o => One_Off_Level_Challenge_Data.From(o)).ToArray();
            num_challenge_prestiges = saveGame.NumChallengePrestiges;
            num_golden_keys_notified = saveGame.NumGoldenKeysNotified;
            packed_weapon_data = saveGame.PackedWeaponData.Select(p => Packed_Weapon_Data.From(p)).ToArray();
            save_game_id = saveGame.SaveGameId;
            received_default_weapon = saveGame.ReceivedDefaultWeapon;
            show_new_playthrough_notification = saveGame.ShowNewPlaythroughNotification;
            inventory_data = Inventory_Data.From(saveGame.InventorySlotData);
            last_saved_date = saveGame.LastSavedDate;
            max_bank_slots = saveGame.MaxBankSlots;
            currency = saveGame.CurrencyOnHand.ToArray();
            completey_explored_areas = saveGame.FullyExploredAreas.ToArray();
            active_mission_number = saveGame.ActiveMissionNumber;
            applied_customizations = saveGame.AppliedCustomizations.ToArray();
            bank_slots = saveGame.BankSlots.Select(s => Bank_Slots.From(s)).ToArray();
            black_market_upgrades = saveGame.BlackMarketUpgrades.ToArray();
            challenge_data = saveGame.ChallengeList.Select(c => Challenge_Data.From(c)).ToArray();
            player_level = saveGame.ExpLevel;
            is_dlc_player_class = saveGame.IsDLCPlayerClass;
            is_badass_mode = saveGame.IsBadassModeSaveGame;
            general_skill_points = saveGame.GeneralSkillPoints;
            player_class = saveGame.PlayerClass;
            dlc_player_class_id = saveGame.DLCPlayerClassPackageId;
            experience_points = saveGame.ExpPoints;
            foreach(var property in GetType()
                .GetProperties()
                .Where(prop => prop.GetValue(this) == null))
            {
                if(property.PropertyType.IsArray)
                {
                    property.SetValue(this, Activator.CreateInstance(property.PropertyType, 0));
                }
                else
                {
                    if (property.PropertyType == typeof(string))
                        property.SetValue(this, "");
                    else
                        property.SetValue(this, Activator.CreateInstance(property.PropertyType));
                }
            }
        }

        public string player_class { get; set; }
        public int player_level { get; set; }
        public int experience_points { get; set; }
        public int general_skill_points { get; set; }
        public int special_skill_points { get; set; }
        public int[] currency { get; set; }
        public int playthroughs_completed { get; set; }
        public Skill_List[] skill_list { get; set; }
        public object[] field9 { get; set; }
        public object[] field10 { get; set; }
        public Resource_Data[] resource_data { get; set; }
        public object[] item_list { get; set; }
        public Inventory_Data inventory_data { get; set; }
        public object[] weapon_data { get; set; }
        public string field15 { get; set; }
        public string[] field16 { get; set; }
        public string field17 { get; set; }
        public Playthrough_Data[] playthrough_data { get; set; }
        public Preferences_Data preferences_data { get; set; }
        public int save_game_id { get; set; }
        public int field21 { get; set; }
        public int field22 { get; set; }
        public int[] field23 { get; set; }
        public int[] field24 { get; set; }
        public int total_play_time { get; set; }
        public string last_saved_date { get; set; }
        public object[] dlc_expansion_data { get; set; }
        public object[] field28 { get; set; }
        public Saved_Regions[] saved_regions { get; set; }
        public World_Discovery_List[] world_discovery_list { get; set; }
        public bool is_badass_mode { get; set; }
        public object[] weapon_mementos { get; set; }
        public object[] item_mementos { get; set; }
        public Save_Guid save_guid { get; set; }
        public string[] applied_customizations { get; set; }
        public int[] black_market_upgrades { get; set; }
        public int active_mission_number { get; set; }
        public Challenge_Data[] challenge_data { get; set; }
        public int[] level_challenge_unlocks { get; set; }
        public One_Off_Level_Challenge_Data[] one_off_level_challenge_data { get; set; }
        public Bank_Slots[] bank_slots { get; set; }
        public int num_challenge_prestiges { get; set; }
        public object[] lockout_data { get; set; }
        public bool is_dlc_player_class { get; set; }
        public int dlc_player_class_id { get; set; }
        public string[] completey_explored_areas { get; set; }
        public object[] golden_keys { get; set; }
        public int num_golden_keys_notified { get; set; }
        public int last_play_through_index { get; set; }
        public bool show_new_playthrough_notification { get; set; }
        public bool received_default_weapon { get; set; }
        public object[] field52 { get; set; }
        public Packed_Item_Data[] packed_item_data { get; set; }
        public Packed_Weapon_Data[] packed_weapon_data { get; set; }
        public bool field55 { get; set; }
        public int max_bank_slots { get; set; }
        public int field57 { get; set; }
        public int field58 { get; set; }
    }

    public class Inventory_Data
    {
        public int inventory_max { get; set; }
        public int weapon_max { get; set; }
        public int num_flourished { get; set; }

        internal static Inventory_Data From(InventorySlotData inventorySlotData)
        {
            return new Inventory_Data()
            {
                inventory_max = inventorySlotData.InventorySlotMax,
                num_flourished = inventorySlotData.NumQuickSlotsFlourished,
                weapon_max = inventorySlotData.WeaponReadyMax
            };
        }

        public static implicit operator InventorySlotData(Inventory_Data data)
        {
            return new InventorySlotData()
            {
                InventorySlotMax = data.inventory_max,
                NumQuickSlotsFlourished = data.num_flourished,
                WeaponReadyMax = data.weapon_max
            };
        }
    }

    public class Preferences_Data
    {
        public string character_name { get; set; }
        public ColorItem primary_color { get; set; }
        public ColorItem secondary_color { get; set; }
        public ColorItem tertiary_color { get; set; }

        internal static Preferences_Data From(UIPreferencesData data)
        {
            return new Preferences_Data()
            {
                character_name = Encoding.UTF8.GetString(data.CharacterName),
                primary_color = ColorItem.From(data.PrimaryColor),
                secondary_color = ColorItem.From(data.SecondaryColor),
                tertiary_color = ColorItem.From(data.TertiaryColor)
            };
        }

        public static implicit operator UIPreferencesData(Preferences_Data data)
        {
            return new UIPreferencesData()
            {
                CharacterName = Encoding.UTF8.GetBytes(data.character_name),
                PrimaryColor = data.primary_color,
                SecondaryColor = data.secondary_color,
                TertiaryColor = data.tertiary_color
            };
        }
    }

    public class ColorItem
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        internal static ColorItem From(Color data)
        {
            return new ColorItem() { A = data.A, B = data.B, G = data.G, R = data.R };
        }

        public static implicit operator Color(ColorItem data)
        {
            return new Color() { A = (byte)data.A, B = (byte)data.B, G = (byte)data.G, R = (byte)data.R };
        }
    }

    public class Save_Guid
    {
        public long A { get; set; }
        public long B { get; set; }
        public long C { get; set; }
        public long D { get; set; }

        internal static Save_Guid From(ProtoBufFormats.WillowTwoSave.Guid data)
        {
            return new Save_Guid()
            {
                A = data.A,
                B = data.B,
                C = data.C,
                D = data.D,
            };
        }

        public static implicit operator ProtoBufFormats.WillowTwoSave.Guid(Save_Guid data)
        {
            return new ProtoBufFormats.WillowTwoSave.Guid()
            {
                A = (uint)data.A,
                B = (uint)data.B,
                C = (uint)data.C,
                D = (uint)data.D,
            };
        }
    }

    public class Skill_List
    {
        public string skill_asset { get; set; }
        public int level { get; set; }
        public int max_points { get; set; }
        public int equipped_slot_id { get; set; }

        internal static Skill_List From(SkillData data)
        {
            return new Skill_List()
            {
                equipped_slot_id = data.EquippedSlotIndex,
                level = data.Grade,
                max_points = data.GradePoints,
                skill_asset = data.Skill
            };
        }

        public static implicit operator SkillData(Skill_List data)
        {
            return new SkillData()
            {
                EquippedSlotIndex = data.equipped_slot_id,
                Grade = data.level,
                GradePoints = data.max_points,
                Skill = data.skill_asset
            };
        }
    }

    public class Resource_Data
    {
        public string resource_asset { get; set; }
        public string field2 { get; set; }
        public float amount { get; set; }
        public int field4 { get; set; }

        internal static Resource_Data From(ResourceData data)
        {
            return new Resource_Data()
            {
                amount = data.Amount,
                resource_asset = data.Resource,
                field2 = data.Pool,
                field4 = data.UpgradeLevel
            };
        }

        public static implicit operator ResourceData(Resource_Data data)
        {
            return new ResourceData()
            {
                Amount = data.amount,
                Resource = data.resource_asset,
                Pool = data.field2,
                UpgradeLevel = data.field4,
            };
        }
    }

    public class Playthrough_Data
    {
        public int number { get; set; }
        public string mission_asset { get; set; }
        public Mission_Data[] mission_data { get; set; }
        public Pending_Rewards[] pending_rewards { get; set; }
        public object[] filterd { get; set; }

        internal static Playthrough_Data From(MissionPlaythroughData data)
        {
            return new Playthrough_Data()
            {
                filterd = new object[0],
                mission_asset = data.ActiveMission,
                number = data.PlayThroughNumber.GetValueOrDefault(),
                mission_data = data.MissionData.Select(m => Mission_Data.From(m)).ToArray(),
                pending_rewards = data.PendingMissionRewards.Select(p => Pending_Rewards.From(p)).ToArray(),
            };
        }

        public static implicit operator MissionPlaythroughData(Playthrough_Data data)
        {
            return new MissionPlaythroughData()
            { 
                FilteredMissions = new List<string>(),
                PlayThroughNumber = data.number,
                ActiveMission = data.mission_asset,
                MissionData = data.mission_data.Select(m => (MissionData)m).ToList(),
                PendingMissionRewards = data.pending_rewards.Select(m => (PendingMissionRewards)m).ToList(),
            };
            
        }
    }

    public class Mission_Data
    {
        public string mission_asset { get; set; }
        public string status { get; set; }
        public bool is_dlc { get; set; }
        public int dlc_package_id { get; set; }
        public int?[] objectives_progress { get; set; }
        public int active_objective_id { get; set; }
        public int?[] sub_objectives_list { get; set; }
        public bool needs_rewards { get; set; }
        public int field9 { get; set; }
        public bool field10 { get; set; }
        public int game_stage { get; set; }

        internal static Mission_Data From(MissionData m)
        {
            return new Mission_Data()
            {
                active_objective_id = m.ActiveObjectiveSetIndex,
                dlc_package_id = m.DLCPackageId,
                field9 = m.Unknown9,
                game_stage = m.GameStage,
                field10 = m.HeardKickoff,
                is_dlc = m.IsFromDLC,
                mission_asset = m.Mission,
                needs_rewards = m.NeedsRewards,
                objectives_progress = m.ObjectivesProgress.Cast<int?>().ToArray(),
                status = "MS_"+m.Status.ToString(),
                sub_objectives_list = m.SubObjectiveSetIndexes.Cast<int?>().ToArray(),
            };
        }

        public static implicit operator MissionData(Mission_Data data)
        {
            return new MissionData()
            {
                ActiveObjectiveSetIndex = data.active_objective_id,
                DLCPackageId = data.dlc_package_id,
                GameStage = data.game_stage,
                IsFromDLC = data.is_dlc,
                Mission = data.mission_asset,
                NeedsRewards = data.needs_rewards,
                Status = (ProtoBufFormats.WillowTwoSave.MissionStatus)Enum.Parse(typeof(ProtoBufFormats.WillowTwoSave.MissionStatus), data.status.Split('_').Last()),
                ObjectivesProgress = data.objectives_progress.Select(s => s.Value).ToList(),
                SubObjectiveSetIndexes = data.sub_objectives_list.Select(s => s.Value).ToList(),
                Unknown9 = data.field9,
                HeardKickoff = data.field10,
            };
        }
    }

    public class Pending_Rewards
    {
        public string mission_asset { get; set; }
        public Weapon_Rewards[] weapon_rewards { get; set; }
        public Item_Rewards[] item_rewards { get; set; }
        public Packed_Weapon_Rewards[] packed_weapon_rewards { get; set; }
        public Packed_Item_Rewards[] packed_item_rewards { get; set; }
        public int field6 { get; set; }
        public int field7 { get; set; }

        internal static Pending_Rewards From(PendingMissionRewards data)
        {
            return new Pending_Rewards()
            {
                field6 = data.IsFromDLC ? 1 : 0,
                field7 = data.DLCPackageId,
                mission_asset = data.Mission,
                item_rewards = data.ItemRewards.Select(p => Item_Rewards.From(p)).ToArray(),
                weapon_rewards = data.WeaponRewards.Select(p => Weapon_Rewards.From(p)).ToArray(),
                packed_item_rewards = data.PackedItemRewards.Select(p => Packed_Item_Rewards.From(p)).ToArray(),
                packed_weapon_rewards = data.PackedWeaponRewards.Select(p => Packed_Weapon_Rewards.From(p)).ToArray(),
            };
        }

        public static implicit operator PendingMissionRewards(Pending_Rewards data)
        {
            return new PendingMissionRewards()
            {
                WeaponRewards = data.weapon_rewards.Select(p => (WeaponData)p).ToList(),
                ItemRewards = data.item_rewards.Select(p => (ItemData)p).ToList(),
                Mission = data.mission_asset,
                IsFromDLC = data.field6 == 1,
                DLCPackageId = data.field7,
                PackedItemRewards = data.packed_item_rewards.Select(p => (PackedItemDataOptional)p).ToList(),
                PackedWeaponRewards = data.packed_weapon_rewards.Select(p => (PackedWeaponDataOptional)p).ToList(),
            };
        }
    }

    public class Weapon_Rewards
    {
        public string weapon_asset { get; set; }
        public string manufacturer { get; set; }
        public string type { get; set; }
        public string body_part { get; set; }
        public string grip_part { get; set; }
        public string barrel_part { get; set; }
        public string sight_part { get; set; }
        public string stock_part { get; set; }
        public string field9 { get; set; }
        public string field10 { get; set; }
        public string field11 { get; set; }
        public string field12 { get; set; }
        public string material_part { get; set; }
        public string prefix_part { get; set; }
        public string title_part { get; set; }
        public int field16 { get; set; }
        public int grade { get; set; }
        public string quick_slot { get; set; }
        public string mark { get; set; }
        public string elemental_part { get; set; }
        public string accessory1_part { get; set; }
        public string accessory2_part { get; set; }

        internal static Weapon_Rewards From(WeaponData data)
        {
            return new Weapon_Rewards()
            {
                accessory1_part = data.Accessory1Part,
                accessory2_part = data.Accessory2Part,
                elemental_part = data.ElementalPart,
                mark = data.Mark.ToString(),
                quick_slot = data.QuickSlot.ToString(),
                grade = data.ManufacturerGradeIndex,
                field16 = data.Unknown16,
                field10 = data.Unknown10,
                field11 = data.Unknown11,
                field12 = data.Unknown12,
                barrel_part = data.BarrelPart,
                body_part = data.BodyPart,
                grip_part = data.GripPart,
                manufacturer = data.Manufacturer,
                material_part = data.MaterialPart,
                prefix_part = data.PrefixPart,
                sight_part = data.SightPart,
                stock_part = data.StockPart,
                title_part = data.TitlePart,
                type = data.Type,
                field9 = data.Unknown9,
                weapon_asset = data.Balance
            };
        }

        public static implicit operator WeaponData(Weapon_Rewards data)
        {
            return new WeaponData()
            {
                Accessory1Part = data.accessory1_part,
                Accessory2Part = data.accessory2_part,
                ElementalPart = data.elemental_part,
                Mark = (PlayerMark)Enum.Parse(typeof(PlayerMark), data.mark),
                QuickSlot = (QuickWeaponSlot)Enum.Parse(typeof(QuickWeaponSlot), data.quick_slot),
                ManufacturerGradeIndex = data.grade,
                Unknown16 = data.field16,
                Unknown10 = data.field10,
                Unknown11 = data.field11,
                Unknown12 = data.field12,
                BarrelPart = data.barrel_part,
                BodyPart = data.body_part,
                GripPart = data.grip_part,
                Manufacturer = data.manufacturer,
                MaterialPart = data.material_part,
                PrefixPart = data.prefix_part,
                SightPart = data.sight_part,
                StockPart = data.stock_part,
                TitlePart = data.title_part,
                Type = data.type,
                Unknown9 = data.field9,
                Balance = data.weapon_asset,

            };
        }
    }


    public class Item_Rewards
    {
        public string item_asset { get; set; }
        public string type { get; set; }
        public string effect_slot1 { get; set; }
        public string effect_slot2 { get; set; }
        public string effect_slot3 { get; set; }
        public string effect_slot4 { get; set; }
        public string effect_slot5 { get; set; }
        public string effect_slot6 { get; set; }
        public string effect_slot7 { get; set; }
        public string effect_slot8 { get; set; }
        public string material_part { get; set; }
        public string manufacturer { get; set; }
        public string prefix_part { get; set; }
        public string title_part { get; set; }
        public int quantity { get; set; }
        public int grade { get; set; }
        public bool is_equipped { get; set; }
        public string mark { get; set; }

        internal static Item_Rewards From(ItemData data)
        {
            return new Item_Rewards()
            {
                effect_slot1 = data.AlphaPart,
                effect_slot2 = data.BetaPart,
                effect_slot3 = data.GammaPart,
                effect_slot4 = data.DeltaPart,
                effect_slot5 = data.EpsilonPart,
                effect_slot6 = data.ZetaPart,
                effect_slot7 = data.EtaPart,
                effect_slot8 = data.ThetaPart,
                mark = data.Mark.ToString(),
                grade = data.ManufacturerGradeIndex,
                manufacturer = data.Manufacturer,
                material_part = data.MaterialPart,
                prefix_part = data.PrefixPart,
                title_part = data.TitlePart,
                type = data.Type,
                item_asset = data.Balance,
                is_equipped = data.Equipped,
                quantity = data.Quantity
            };
        }

        public static implicit operator ItemData(Item_Rewards data)
        {
            return new ItemData()
            {
                AlphaPart = data.effect_slot1,
                BetaPart = data.effect_slot2,
                GammaPart = data.effect_slot3,
                DeltaPart = data.effect_slot4,
                EpsilonPart = data.effect_slot5,
                ZetaPart = data.effect_slot6,
                EtaPart = data.effect_slot7,
                ThetaPart = data.effect_slot8,
                Mark = (PlayerMark)Enum.Parse(typeof(PlayerMark), data.mark),
                ManufacturerGradeIndex = data.grade,
                Manufacturer = data.manufacturer,
                MaterialPart = data.material_part,
                PrefixPart = data.prefix_part,
                TitlePart = data.title_part,
                Type = data.type,
                Balance = data.item_asset,
                Equipped = data.is_equipped,
                Quantity = data.quantity

            };

        }
    }

        public class Packed_Weapon_Rewards
    {
        public string weapon_serial_number { get; set; }
        public string quick_slot { get; set; }
        public string mark { get; set; }
        public int field4 { get; set; }

        internal static Packed_Weapon_Rewards From(PackedWeaponDataOptional p)
        {
            return new Packed_Weapon_Rewards()
            {
                quick_slot = p.QuickSlot.ToString(),
                weapon_serial_number = Convert.ToBase64String(p.InventorySerialNumber),
                mark = p.Mark.ToString(),
                field4 = p.Unknown4
            };
        }

        public static implicit operator PackedWeaponDataOptional(Packed_Weapon_Rewards data)
        {
            var item = new PackedWeaponDataOptional()
            {
                QuickSlot = (QuickWeaponSlot)Enum.Parse(typeof(QuickWeaponSlot), data.quick_slot),
                InventorySerialNumber = Convert.FromBase64String(data.weapon_serial_number),
                Unknown4 = data.field4,
            };
            switch (data.mark)
            {
                case "Trash":
                    item.Mark = PlayerMark.Trash;
                    break;
                case "Standard":
                    item.Mark = PlayerMark.Standard;
                    break;
                case "Favorite":
                    item.Mark = PlayerMark.Favorite;
                    break;
            }
            return item;
        }
    }

    public class Packed_Item_Rewards
    {
        public string item_serial_number { get; set; }
        public int quantity { get; set; }
        public bool is_equipped { get; set; }
        public string mark { get; set; }

        internal static Packed_Item_Rewards From(PackedItemDataOptional p)
        {
            return new Packed_Item_Rewards()
            {
                is_equipped = p.Equipped,
                item_serial_number = Convert.ToBase64String(p.InventorySerialNumber),
                mark = p.Mark.ToString(),
                quantity = p.Quantity
            };
        }

        public static implicit operator PackedItemDataOptional(Packed_Item_Rewards data)
        {
            var item = new PackedItemDataOptional()
            {
                Equipped = data.is_equipped,
                InventorySerialNumber = Convert.FromBase64String(data.item_serial_number),
                Quantity = data.quantity,
            };
            switch (data.mark)
            {
                case "Trash":
                    item.Mark = PlayerMark.Trash;
                    break;
                case "Standard":
                    item.Mark = PlayerMark.Standard;
                    break;
                case "Favorite":
                    item.Mark = PlayerMark.Favorite;
                    break;
            }
            return item;
        }
    }

    public class Saved_Regions
    {
        public string region_asset { get; set; }
        public int game_stage { get; set; }
        public int play_through_idx { get; set; }
        public int field4 { get; set; }
        public int field5 { get; set; }
        public static explicit operator RegionGameStageData(Saved_Regions data)
        {
            return new RegionGameStageData()
            {
                Region = data.region_asset,
                GameStage = data.game_stage,
                PlaythroughIndex = data.play_through_idx,
                DLCPackageId = data.field4,
                IsFromDLC = data.field5 == 1,
            };
        }

        public static Saved_Regions From(RegionGameStageData data)
        {
            return new Saved_Regions()
            {
                region_asset = data.Region,
                game_stage = data.GameStage,
                play_through_idx = data.PlaythroughIndex,
                field4 = data.DLCPackageId,
                field5 = data.IsFromDLC ? 1 : 0,
            };
        }
    }

    public class World_Discovery_List
    {
        public string world_asset { get; set; }
        public bool is_uncovered { get; set; }

        internal static World_Discovery_List From(WorldDiscoveryData data)
        {
            return new World_Discovery_List()
            {
                world_asset = data.DiscoveryName,
                is_uncovered = data.HasBeenUncovered,
            };
        }

        public static implicit operator WorldDiscoveryData(World_Discovery_List data)
        {
            return new WorldDiscoveryData()
            {
                DiscoveryName = data.world_asset,
                HasBeenUncovered = data.is_uncovered
            };
        }
    }

    public class Challenge_Data
    {
        public string challenge_asset { get; set; }
        public int field2 { get; set; }
        public int field3 { get; set; }

        internal static Challenge_Data From(ChallengeData c)
        {
            return new Challenge_Data()
            { challenge_asset = c.Challenge, field2 = c.IsFromDLC ? 1 : 0, field3 = c.DLCPackageId };
        }

        public static implicit operator ChallengeData(Challenge_Data data)
        {
            return new ChallengeData()
            { Challenge = data.challenge_asset, DLCPackageId = data.field3, IsFromDLC = data.field2 == 1 };
        }
    }

    public class One_Off_Level_Challenge_Data
    {
        public int package_id { get; set; }
        public int content_id { get; set; }
        public long[] competed { get; set; }

        internal static One_Off_Level_Challenge_Data From(OneOffLevelChallengeData o)
        {
            return new One_Off_Level_Challenge_Data()
            {
                content_id = o.ContentId,
                package_id = o.PackageId,
                competed = o.Completion.Select(c => (long)c).ToArray()
            };
        }

        public static implicit operator OneOffLevelChallengeData(One_Off_Level_Challenge_Data data)
        {
            return new OneOffLevelChallengeData()
            {
                Completion = data.competed.Select(c => (uint)c).ToList(),
                ContentId = data.content_id,
                PackageId = data.package_id,
            };
        }

    }

    public class Bank_Slots
    {
        public string field1 { get; set; }

        internal static Bank_Slots From(BankSlot s)
        {
            return new Bank_Slots() { field1 = Convert.ToBase64String(s.InventorySerialNumber) };
        }

        public static implicit operator BankSlot(Bank_Slots slot)
        {
            return new BankSlot() { InventorySerialNumber = Convert.FromBase64String(slot.field1) };
        }
    }

    public class Packed_Item_Data
    {
        public string item_serial_number { get; set; }
        public int quantity { get; set; }
        public bool is_equipped { get; set; }
        public string mark { get; set; }

        internal static Packed_Item_Data From(PackedItemData i)
        {
            return new Packed_Item_Data()
            {
                is_equipped = i.Equipped,
                item_serial_number = Convert.ToBase64String(i.InventorySerialNumber),
                mark = ((PlayerMark)i.Mark).ToString(),
                quantity = i.Quantity
            };
        }

        public static implicit operator PackedItemData(Packed_Item_Data data)
        {
            var item = new PackedItemData()
            {
                Equipped = data.is_equipped,
                InventorySerialNumber = Convert.FromBase64String(data.item_serial_number),
                Quantity = data.quantity
            };
            switch (data.mark)
            {
                case "Trash":
                    item.Mark = (int)PlayerMark.Trash;
                    break;
                case "Standard":
                    item.Mark = (int)PlayerMark.Standard;
                    break;
                case "Favorite":
                    item.Mark = (int)PlayerMark.Favorite;
                    break;
            }
            return item;
        }
    }

    public class Packed_Weapon_Data
    {
        public string weapon_serial_number { get; set; }
        public string quick_slot { get; set; }
        public string mark { get; set; }
        public int field4 { get; set; }

        internal static Packed_Weapon_Data From(PackedWeaponData p)
        {
            return new Packed_Weapon_Data()
            {
                field4 = p.Unknown4,
                quick_slot = p.QuickSlot.ToString(),
                mark = p.Mark.ToString(),
                weapon_serial_number = Convert.ToBase64String(p.InventorySerialNumber)
            };
        }

        public static implicit operator PackedWeaponData(Packed_Weapon_Data data)
        {
            var item = new PackedWeaponData()
            {
                QuickSlot = (QuickWeaponSlot)Enum.Parse(typeof(QuickWeaponSlot), data.quick_slot),
                InventorySerialNumber = Convert.FromBase64String(data.weapon_serial_number),
                Unknown4 = data.field4,
            };
            switch (data.mark)
            {
                case "Trash":
                    item.Mark = PlayerMark.Trash;
                    break;
                case "Standard":
                    item.Mark = PlayerMark.Standard;
                    break;
                case "Favorite":
                    item.Mark = PlayerMark.Favorite;
                    break;
            }
            return item;
        }
    }

}

