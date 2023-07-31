using Caliburn.Micro;

using Gibbed.Borderlands2.FileFormats;
using Gibbed.Borderlands2.GameInfo;
using Gibbed.Borderlands2.ProtoBufFormats.WillowTwoSave;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gibbed.Borderlands2.SaveEdit.Mission
{
    internal class BaseMissionViewModel : PropertyChangedBase
    {
        private static readonly Brush NoEntryBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 245, 245));
        private static readonly Brush AvailEntryBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 236, 0));
        private string _DisplayName = "";
        private MissionData _Mission;
        private PendingMissionRewards _Rewards;
        private Platform _Platform;
        private int _Playthrough;
        private Brush _Color;

        public BaseMissionViewModel(string displayName,
                                    MissionData mission,
                                    Platform platform,
                                    int playthrough,
                                    PendingMissionRewards rewards = null)
        {
            _DisplayName = displayName;
            _Mission = mission;
            if(rewards == null)
            {
                _Rewards = new PendingMissionRewards()
                {
                    IsFromDLC = mission.IsFromDLC,
                    DLCPackageId = mission.DLCPackageId,
                    Mission = mission.Mission,
                    PackedItemRewards = new List<PackedItemDataOptional>(),
                    PackedWeaponRewards = new List<PackedWeaponDataOptional>(),
                    ItemRewards = new List<ItemData>(),
                    WeaponRewards = new List<WeaponData>()
                };
                _Color = NoEntryBrush;
            }
            else
            {
                _Rewards = rewards;
                _Color = AvailEntryBrush;
            }
            _Platform = platform;
            _Playthrough = playthrough;
        }

        public void UpdateState(bool isEmpty)
        {
            BgColor = isEmpty ? NoEntryBrush : AvailEntryBrush;
        }

        internal void ClearAll()
        {
            Reward.WeaponRewards.Clear();
            Reward.ItemRewards.Clear();
        }

        public virtual string DisplayName
        {
            get { return this._DisplayName; }
            private set
            {
                this._DisplayName = value;
                this.NotifyOfPropertyChange(nameof(DisplayName));
            }
        }

        public Brush BgColor
        {
            get => _Color;
            set
            {
                _Color = value;
                this.NotifyOfPropertyChange(nameof(BgColor));
            }
        }
        public string DisplayGroup
        {
            get { return $"Playthrough[{_Playthrough}]"; }
        }

        public Platform Platform => _Platform;
        public PendingMissionRewards Reward => _Rewards;
        public int Playthrough => _Playthrough;
        //public bool IsEmpty => Reward.PackedItemRewards.Count == 0 && Reward.PackedWeaponRewards.Count == 0;
        public bool IsEmpty => Reward.ItemRewards.Count == 0 && Reward.WeaponRewards.Count == 0;

    }
}
