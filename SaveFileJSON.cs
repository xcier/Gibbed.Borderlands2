using System;

public class SaveFileJSON
{
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
    public object[] field24 { get; set; }
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
    public object[] level_challenge_unlocks { get; set; }
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
}

public class Preferences_Data
{
    public string character_name { get; set; }
    public Primary_Color primary_color { get; set; }
    public Secondary_Color secondary_color { get; set; }
    public Tertiary_Color tertiary_color { get; set; }
}

public class Primary_Color
{
    public int A { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
}

public class Secondary_Color
{
    public int A { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
}

public class Tertiary_Color
{
    public int A { get; set; }
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
}

public class Save_Guid
{
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
    public int D { get; set; }
}

public class Skill_List
{
    public string skill_asset { get; set; }
    public int level { get; set; }
    public int max_points { get; set; }
    public int equipped_slot_id { get; set; }
}

public class Resource_Data
{
    public string resource_asset { get; set; }
    public string field2 { get; set; }
    public float amount { get; set; }
    public int field4 { get; set; }
}

public class Playthrough_Data
{
    public int number { get; set; }
    public string mission_asset { get; set; }
    public Mission_Data[] mission_data { get; set; }
    public Pending_Rewards[] pending_rewards { get; set; }
    public object[] filterd { get; set; }
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
}

public class Pending_Rewards
{
    public string mission_asset { get; set; }
    public object[] weapon_rewards { get; set; }
    public object[] item_rewards { get; set; }
    public Packed_Weapon_Rewards[] packed_weapon_rewards { get; set; }
    public Packed_Item_Rewards[] packed_item_rewards { get; set; }
    public int field6 { get; set; }
    public int field7 { get; set; }
}

public class Packed_Weapon_Rewards
{
    public string weapon_serial_number { get; set; }
    public string quick_slot { get; set; }
    public string mark { get; set; }
    public int field4 { get; set; }
}

public class Packed_Item_Rewards
{
    public string item_serial_number { get; set; }
    public int quantity { get; set; }
    public bool is_equipped { get; set; }
    public string mark { get; set; }
}

public class Saved_Regions
{
    public string region_asset { get; set; }
    public int game_stage { get; set; }
    public int play_through_idx { get; set; }
    public int field4 { get; set; }
    public int field5 { get; set; }
}

public class World_Discovery_List
{
    public string world_asset { get; set; }
    public bool is_uncovered { get; set; }
}

public class Challenge_Data
{
    public string challenge_asset { get; set; }
    public int field2 { get; set; }
    public int field3 { get; set; }
}

public class One_Off_Level_Challenge_Data
{
    public int package_id { get; set; }
    public int content_id { get; set; }
    public int[] competed { get; set; }
}

public class Bank_Slots
{
    public string field1 { get; set; }
}

public class Packed_Item_Data
{
    public string item_serial_number { get; set; }
    public int quantity { get; set; }
    public bool is_equipped { get; set; }
    public string mark { get; set; }
}

public class Packed_Weapon_Data
{
    public string weapon_serial_number { get; set; }
    public string quick_slot { get; set; }
    public string mark { get; set; }
    public int field4 { get; set; }
}

}
