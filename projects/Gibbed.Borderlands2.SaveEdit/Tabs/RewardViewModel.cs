using Caliburn.Micro;
using Caliburn.Micro.Contrib.Results;

using Gibbed.Borderlands2.FileFormats;
using Gibbed.Borderlands2.FileFormats.Items;
using Gibbed.Borderlands2.GameInfo;
using Gibbed.Borderlands2.ProtoBufFormats.WillowTwoSave;
using Gibbed.Gearbox.WPF;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Gibbed.Borderlands2.SaveEdit.Mission;
using ICSharpCode.SharpZipLib.Zip;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Newtonsoft.Json.Linq;

namespace Gibbed.Borderlands2.SaveEdit
{
    [Export(typeof(RewardViewModel))]
    internal class RewardViewModel : PropertyChangedBase
    {

        private static readonly DownloadablePackageDefinition[] _DefaultDownloadablePackages;

        static RewardViewModel()
        {
            _DefaultDownloadablePackages = new[] { DownloadablePackageDefinition.Default };
        }

        #region Imports
        private CharacterViewModel _Character;
        private BackpackViewModel _Backpack;

        [Import(typeof(CharacterViewModel))]
        public CharacterViewModel Character
        {
            get { return this._Character; }
            set
            {
                this._Character = value;
                this.NotifyOfPropertyChange(nameof(Character));
            }
        }

        [Import(typeof(BackpackViewModel))]
        public BackpackViewModel Backpack
        {
            get { return this._Backpack; }
            set
            {
                this._Backpack = value;
                this.NotifyOfPropertyChange(nameof(Backpack));
            }
        }
        #endregion

        #region Fields
        private readonly ObservableCollection<BaseMissionViewModel> _Missions;
        private readonly Dictionary<BaseMissionViewModel, IBaseSlotViewModel[]> _CachedSlots;
        private readonly ObservableCollection<IBaseSlotViewModel> _Slots;

        private readonly List<KeyValuePair<BankSlot, Exception>> _BrokenSlots;

        private BaseMissionViewModel _SelectedMission;

        private IBaseSlotViewModel _SelectedSlot;

        private readonly ICommand _NewWeapon;
        private bool _NewWeaponDropDownIsOpen;
        private readonly ICommand _NewItem;
        private readonly (int key, string asset)[] _CachedAssets;
        private bool _NewItemDropDownIsOpen;
        #endregion

        #region Properties
        public IEnumerable<DownloadablePackageDefinition> DownloadablePackages
        {
            get
            {
                return _DefaultDownloadablePackages.Concat(
                    InfoManager.DownloadableContents.Items
                               .Where(dc => dc.Value.Type == DownloadableContentType.ItemSet &&
                                            dc.Value.Package != null)
                               .Select(dc => dc.Value.Package)
                               .Where(dp => InfoManager.AssetLibraryManager.Sets.Any(s => s.Id == dp.Id) == true)
                               .Distinct()
                               .OrderBy(dp => dp.Id));
            }
        }

        public bool HasDownloadablePackages
        {
            get { return this.DownloadablePackages.Any(); }
        }

        public ObservableCollection<BaseMissionViewModel> Missions
        {
            get { return this._Missions; }
        }
        public ObservableCollection<IBaseSlotViewModel> Slots
        {
            get { return this._Slots; }
        }


        public List<KeyValuePair<BankSlot, Exception>> BrokenSlots
        {
            get { return this._BrokenSlots; }
        }

        public IBaseSlotViewModel SelectedSlot
        {
            get { return this._SelectedSlot; }
            set
            {
                this._SelectedSlot = value;
                this.NotifyOfPropertyChange(nameof(SelectedSlot));
            }
        }

        public BaseMissionViewModel  SelectedMission
        {
            get { return this._SelectedMission; }
            set
            {
                var previous = this._SelectedMission;
                this._SelectedMission = value;
                LoadRewards(value, previous);
                this.NotifyOfPropertyChange(nameof(SelectedMission));
            }
        }

        public ICommand NewWeapon
        {
            get { return this._NewWeapon; }
        }

        public bool NewWeaponDropDownIsOpen
        {
            get { return this._NewWeaponDropDownIsOpen; }
            set
            {
                this._NewWeaponDropDownIsOpen = value;
                this.NotifyOfPropertyChange(nameof(NewWeaponDropDownIsOpen));
            }
        }

        public ICommand NewItem
        {
            get { return this._NewItem; }
        }

        public bool NewItemDropDownIsOpen
        {
            get { return this._NewItemDropDownIsOpen; }
            set
            {
                this._NewItemDropDownIsOpen = value;
                this.NotifyOfPropertyChange(nameof(NewItemDropDownIsOpen));
            }
        }
        #endregion

        [ImportingConstructor]
        public RewardViewModel()
        {
            this._Missions = new ObservableCollection<BaseMissionViewModel>();
            this._CachedSlots = new Dictionary<BaseMissionViewModel, IBaseSlotViewModel[]>();
            this._Slots = new ObservableCollection<IBaseSlotViewModel>();
            this.Slots.CollectionChanged += Slots_CollectionChanged;
            this._BrokenSlots = new List<KeyValuePair<BankSlot, Exception>>();
            this._NewWeapon = new DelegateCommand<int>(this.DoNewWeapon);
            this._NewItem = new DelegateCommand<int>(this.DoNewItem);
            this._CachedAssets = InfoManager.AssetLibraryManager.Sets.GroupBy(set => set.Id, set =>
                {
                    return set.Libraries.Values.SelectMany(lib => lib.Sublibraries.SelectMany(sublib => sublib.Assets));
                })
                .Aggregate(new List<(int key, string asset)>(), (acc, group) =>
                {
                    foreach(var value in group.SelectMany(assets => assets))
                    {
                        acc.Add((group.Key, value));
                    }
                    return acc;
                })
                .ToArray();
        }

        public void DoNewWeapon(int assetLibrarySetId)
        {
            var weapon = new BaseWeapon()
            {
                UniqueId = new Random().Next(int.MinValue, int.MaxValue),
                // TODO: check other item unique IDs to prevent rare collisions
                AssetLibrarySetId = assetLibrarySetId,
            };
            var viewModel = new BaseWeaponViewModel(weapon);
            this.Slots.Add(viewModel);
            this.SelectedSlot = viewModel;
            this.NewWeaponDropDownIsOpen = false;
        }

        public void DoNewItem(int assetLibrarySetId)
        {
            var item = new BaseItem()
            {
                UniqueId = new Random().Next(int.MinValue, int.MaxValue),
                // TODO: check other item unique IDs to prevent rare collisions
                AssetLibrarySetId = assetLibrarySetId,
            };
            var viewModel = new BaseItemViewModel(item);
            this.Slots.Add(viewModel);
            this.SelectedSlot = viewModel;
            this.NewItemDropDownIsOpen = false;
        }

        private static readonly Regex _CodeSignature =
            new Regex(@"BL2\((?<data>(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?)\)",
                      RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public IEnumerable<IResult> PasteCode()
        {
            bool containsUnicodeText = false;
            if (MyClipboard.ContainsText(TextDataFormat.Text, out var containsText) != MyClipboard.Result.Success ||
                MyClipboard.ContainsText(TextDataFormat.UnicodeText, out containsUnicodeText) !=
                MyClipboard.Result.Success)
            {
                yield return new MyMessageBox("Clipboard failure.", "Error")
                    .WithIcon(MessageBoxImage.Error);
            }

            if (containsText == false &&
                containsUnicodeText == false)
            {
                yield break;
            }

            var errors = 0;
            var viewModels = new List<IBaseSlotViewModel>();
            yield return new DelegateResult(
                () =>
                {
                    if (MyClipboard.GetText(out var codes) != MyClipboard.Result.Success)
                    {
                        MessageBox.Show(
                            "Clipboard failure.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    // strip whitespace
                    codes = Regex.Replace(codes, @"\s+", "");

                    foreach (var match in _CodeSignature.Matches(codes).Cast<Match>()
                                                        .Where(m => m.Success == true))
                    {
                        var code = match.Groups["data"].Value;

                        IPackableSlot packable;

                        try
                        {
                            var data = Convert.FromBase64String(code);
                            packable = BaseDataHelper.Decode(data, Platform.PC);
                        }
                        catch (Exception)
                        {
                            errors++;
                            continue;
                        }

                        // TODO: check other item unique IDs to prevent rare collisions
                        packable.UniqueId = new Random().Next(int.MinValue, int.MaxValue);

                        if (packable is BaseWeapon weapon)
                        {
                            var viewModel = new BaseWeaponViewModel(weapon);
                            viewModels.Add(viewModel);
                        }
                        else if (packable is BaseItem item)
                        {
                            var viewModel = new BaseItemViewModel(item);
                            viewModels.Add(viewModel);
                        }
                    }
                });

            if (viewModels.Count > 0)
            {
                viewModels.ForEach(vm => this.Slots.Add(vm));
                this.SelectedSlot = viewModels.First();
            }

            if (errors > 0)
            {
                yield return
                    new MyMessageBox($"Failed to load {errors} codes.", "Warning")
                        .WithIcon(MessageBoxImage.Warning);
            }
            else if (viewModels.Count == 0)
            {
                yield return
                    new MyMessageBox(
                        "Did not find any codes in clipboard.",
                        "Warning")
                        .WithIcon(MessageBoxImage.Warning);
            }
        }

        public IEnumerable<IResult> CopySelectedSlotCode()
        {
            yield return new DelegateResult(
                () =>
                {
                    if (this.SelectedSlot == null ||
                        this.SelectedSlot.BaseSlot == null)
                    {
                        if (MyClipboard.SetText("") != MyClipboard.Result.Success)
                        {
                            MessageBox.Show(
                                "Clipboard failure.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                        return;
                    }

                    // just a hack until I add a way to override the unique ID in Encode()
                    var copy = (IPackableSlot)this.SelectedSlot.BaseSlot.Clone();
                    copy.UniqueId = 0;

                    var data = BaseDataHelper.Encode(copy, Platform.PC);
                    var sb = new StringBuilder();
                    sb.Append("BL2(");
                    sb.Append(Convert.ToBase64String(data, Base64FormattingOptions.None));
                    sb.Append(")");

                    if (MyClipboard.SetText(sb.ToString()) != MyClipboard.Result.Success)
                    {
                        MessageBox.Show(
                            "Clipboard failure.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                });
        }

        public void DuplicateSelectedSlot()
        {
            if (this.SelectedSlot == null)
            {
                return;
            }

            var copy = (IPackableSlot)this.SelectedSlot.BaseSlot.Clone();
            copy.UniqueId = new Random().Next(int.MinValue, int.MaxValue);
            // TODO: check other item unique IDs to prevent rare collisions

            if (copy is BaseWeapon weapon)
            {
                var viewModel = new BaseWeaponViewModel(weapon);
                this.Slots.Add(viewModel);
                this.SelectedSlot = viewModel;
            }
            else if (copy is BaseItem item)
            {
                var viewModel = new BaseItemViewModel(item);
                this.Slots.Add(viewModel);
                this.SelectedSlot = viewModel;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void UnbankSelectedSlot()
        {
            if (this.SelectedSlot == null)
            {
                return;
            }

            var slot = this.SelectedSlot.BaseSlot;
            this.Slots.Remove(this.SelectedSlot);

            if (slot is BaseWeapon weapon)
            {
                this.Backpack.Slots.Add(new BackpackWeaponViewModel(new BackpackWeapon(weapon)));
            }
            else if (slot is BaseItem item)
            {
                this.Backpack.Slots.Add(new BackpackItemViewModel(new BackpackItem(item)));
            }
        }

        public void DeleteSelectedSlot()
        {
            if (this.SelectedSlot == null)
            {
                return;
            }

            this.Slots.Remove(this.SelectedSlot);
            this.SelectedSlot = this.Slots.FirstOrDefault();
        }

        public void SyncAllLevels()
        {
            foreach (var viewModel in this.Slots)
            {
                if (viewModel is BaseWeaponViewModel weapon)
                {
                    if ((weapon.ManufacturerGradeIndex + weapon.GameStage) >= 2)
                    {
                        weapon.ManufacturerGradeIndex = this.Character.SyncLevel;
                        weapon.GameStage = this.Character.SyncLevel;
                    }
                }
                else if (viewModel is BaseItemViewModel item)
                {
                    if ((item.ManufacturerGradeIndex + item.GameStage) >= 2)
                    {
                        item.ManufacturerGradeIndex = this.Character.SyncLevel;
                        item.GameStage = this.Character.SyncLevel;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public void ImportData(WillowTwoPlayerSaveGame saveGame, Platform platform)
        {
            this._Missions.Clear();
            this._CachedSlots.Clear();
            this.Slots.Clear();
            this._BrokenSlots.Clear();
            for (int playthroughNum = 0; playthroughNum < saveGame.MissionPlaythroughs.Count; playthroughNum++)
            {
                var copies = new Dictionary<string, int>();
                var playthrough = saveGame.MissionPlaythroughs[playthroughNum];
                foreach (var mission in playthrough.MissionData)
                {
                    var rewards = playthrough.PendingMissionRewards.FirstOrDefault(r => r.Mission == mission.Mission && (r.WeaponRewards.Count != 0 || r.ItemRewards.Count != 0) );
                    copies.TryGetValue(mission.Mission, out var num);
                    this._Missions.Add(new BaseMissionViewModel(mission.Mission  + $"[{num}]", mission, platform, playthroughNum, rewards));
                    copies[mission.Mission] = num+1;
                }
            }
            SelectedMission = this._Missions.FirstOrDefault();
        }

        private void LoadRewards(BaseMissionViewModel mission, BaseMissionViewModel previous)
        {
            if(previous != null)
            {
                this._CachedSlots[previous] = this.Slots.ToArray();
            }
            this.Slots.Clear();
            this._BrokenSlots.Clear();
            if (mission == null)
            {
                this.SelectedSlot = null;
                return;
            }
            
            var rewards = mission.Reward;
            var platform = mission.Platform;
            if(this._CachedSlots.TryGetValue(SelectedMission, out var slots))
            {
                foreach (var slot in slots)
                {
                    this._Slots.Add(slot);
                }
            }
            else
            {
                LoadUnpackedRewards(mission);
                //LoadReward(rewards.PackedWeaponRewards.Select(r => r.InventorySerialNumber), platform);
                //LoadReward(rewards.PackedItemRewards.Select(r => r.InventorySerialNumber), platform);
                this._CachedSlots[SelectedMission] = this._Slots.ToArray();
            }
            this.SelectedSlot = this.Slots.FirstOrDefault();
        }

        private void Slots_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SelectedMission?.UpdateState(this.Slots.Count == 0);
        }

        private void LoadUnpackedRewards(BaseMissionViewModel missionVm)
        {
            var rand = new Random();
            
            var itemsRewards =  missionVm.Reward.ItemRewards.Select(item =>
            {
                var parts = item.Type.Split(new[] { '.' }, 2);
                var asset = parts?.Length >= 2 ? parts[1] : "";
                return new BaseItem()
                {
                    AssetLibrarySetId = this._CachedAssets.FirstOrDefault(c => c.asset == asset).key,
                    UniqueId = rand.Next(int.MinValue, int.MaxValue),
                    ManufacturerGradeIndex = item.ManufacturerGradeIndex,
                    AlphaPart = item.AlphaPart,
                    BetaPart = item.BetaPart,
                    DeltaPart = item.DeltaPart,
                    EpsilonPart = item.EpsilonPart,
                    EtaPart = item.EtaPart,
                    GammaPart = item.GammaPart,
                    Manufacturer = item.Manufacturer,
                    MaterialPart = item.MaterialPart,
                    PrefixPart = item.PrefixPart,
                    ThetaPart = item.ThetaPart,
                    TitlePart = item.TitlePart,
                    ZetaPart = item.ZetaPart,
                    Balance = item.Balance,
                    Item = item.Type,
                };
            }).ToArray();
            var weaponRewards = missionVm.Reward.WeaponRewards.Select(item =>
            {
                var parts = item.Type.Split(new[] { '.' }, 2);
                var asset = parts?.Length >= 2 ? parts[1] : "";
                return new BaseWeapon()
                {
                    AssetLibrarySetId = this._CachedAssets.FirstOrDefault(c => c.asset == asset).key,
                    UniqueId = rand.Next(int.MinValue, int.MaxValue),
                    Accessory1Part = item.Accessory1Part,
                    Accessory2Part = item.Accessory2Part,
                    Balance = item.Balance,
                    BarrelPart = item.BarrelPart,
                    BodyPart = item.BodyPart,
                    ElementalPart = item.ElementalPart,
                    GripPart = item.GripPart,
                    Manufacturer = item.Manufacturer,
                    ManufacturerGradeIndex = item.ManufacturerGradeIndex,
                    MaterialPart = item.MaterialPart,
                    PrefixPart = item.PrefixPart,
                    SightPart = item.SightPart,
                    StockPart = item.StockPart,
                    TitlePart = item.TitlePart,
                    WeaponType = item.Type,

                };
                }).ToArray();
            foreach(var weapon in weaponRewards)
            {
                if(string.IsNullOrEmpty(weapon.PrefixPart))
                {
                    weapon.PrefixPart = "None";
                }

                if (string.IsNullOrEmpty(weapon.TitlePart))
                {
                    weapon.TitlePart = "None";
                }
                this.Slots.Add(new BaseWeaponViewModel(weapon));
            }
            foreach(var item in itemsRewards)
            {
                if (string.IsNullOrEmpty(item.PrefixPart))
                {
                    item.PrefixPart = "None";
                }

                if (string.IsNullOrEmpty(item.TitlePart))
                {
                    item.TitlePart = "None";
                }
                this.Slots.Add(new BaseItemViewModel(item));
            }
        }

        private void LoadReward(IEnumerable<byte[]> serials, Platform platform)
        {
            foreach (var serial in serials)
            {
                IPackableSlot slot;
                try
                {
                    slot = BaseDataHelper.Decode(serial, platform);
                }
                catch (Exception e)
                {
                    //this._BrokenSlots.Add(new KeyValuePair<BankSlot, Exception>(bankSlot, e));
                    continue;
                }

                var test = BaseDataHelper.Encode(slot, platform);
                if (serial.SequenceEqual(test) == false)
                {
                    throw new FormatException("bank slot reencode mismatch");
                }

                if (slot is BaseWeapon weapon)
                {
                    var viewModel = new BaseWeaponViewModel(weapon);
                    this.Slots.Add(viewModel);
                }
                else if (slot is BaseItem item)
                {
                    var viewModel = new BaseItemViewModel(item);
                    this.Slots.Add(viewModel);
                }
            }
        }



        public void ExportData(WillowTwoPlayerSaveGame saveGame, Platform platform)
        {
            if(SelectedMission != null)
            {
                this._CachedSlots[SelectedMission] = this._Slots.ToArray();
            }
            foreach (var playthrough in saveGame.MissionPlaythroughs)
            {
                playthrough.PendingMissionRewards.Clear();
            }
            foreach (var mission in Missions)
            {
                var playthrough = saveGame.MissionPlaythroughs[mission.Playthrough];
                if(this._CachedSlots.TryGetValue(mission, out var slots))
                {
                    mission.ClearAll();
                    foreach (var viewModel in slots)
                    {
                        var slot = viewModel.BaseSlot;

                        if (slot is BaseWeapon weapon)
                        {
                            //var data = BaseDataHelper.Encode(weapon, platform);
                            //mission.Reward.PackedWeaponRewards.Add(new PackedWeaponDataOptional()
                            //{
                            //    InventorySerialNumber = data,
                            //    Mark = PlayerMark.Trash,
                            //    QuickSlot = 0,
                            //    Unknown4 = 0,
                            //});
                            mission.Reward.WeaponRewards.Add(new WeaponData()
                            {
                                Accessory1Part = weapon.Accessory1Part,
                                Accessory2Part = weapon.Accessory2Part,
                                Balance = weapon.Balance,
                                BarrelPart = weapon.BarrelPart,
                                BodyPart = weapon.BodyPart,
                                ElementalPart = weapon.ElementalPart,
                                GripPart = weapon.GripPart,
                                Manufacturer = weapon.Manufacturer,
                                ManufacturerGradeIndex = weapon.ManufacturerGradeIndex,
                                MaterialPart = weapon.MaterialPart,
                                PrefixPart = weapon.PrefixPart,
                                SightPart = weapon.SightPart,
                                StockPart = weapon.StockPart,
                                TitlePart = weapon.TitlePart,
                                Type = weapon.WeaponType,
                                Mark = PlayerMark.Trash,
                                QuickSlot = 0,
                                Unknown10 = "",
                                Unknown11 = "",
                                Unknown12 = "",
                                Unknown9 = "",
                                Unknown16 = 0,
                            });
                        }
                        else if (slot is BaseItem item)
                        {
                            //var data = BaseDataHelper.Encode(item, platform);
                            //mission.Reward.PackedItemRewards.Add(new PackedItemDataOptional()
                            //{
                            //    InventorySerialNumber = data,
                            //    Mark = PlayerMark.Trash,
                            //    Equipped = false,
                            //    Quantity = 0,
                            //});

                            mission.Reward.ItemRewards.Add(new ItemData()
                            {
                                ManufacturerGradeIndex = item.ManufacturerGradeIndex,
                                AlphaPart = item.AlphaPart,
                                BetaPart = item.BetaPart,
                                DeltaPart = item.DeltaPart,
                                EpsilonPart = item.EpsilonPart,
                                EtaPart = item.EtaPart,
                                GammaPart = item.GammaPart,
                                Manufacturer = item.Manufacturer,
                                MaterialPart = item.MaterialPart,
                                PrefixPart = item.PrefixPart,
                                ThetaPart = item.ThetaPart,
                                TitlePart = item.TitlePart,
                                ZetaPart = item.ZetaPart,
                                Balance = item.Balance,
                                Type = item.Item,
                                Mark = PlayerMark.Trash,
                                Equipped = false,
                                Quantity = 0,
                            });
                        }
                    }

                }
                if(!mission.IsEmpty)
                {
                    playthrough.PendingMissionRewards.Add(mission.Reward);
                }
            }

            //this._BrokenSlots.ForEach(kv => saveGame.BankSlots.Add(kv.Key));
        }
    }
}
