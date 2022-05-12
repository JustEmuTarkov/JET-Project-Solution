using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinglePlayerMod.Patches.Raid.Airdrop
{
    public class Config
    {
        public ChancePercent airdropChancePercent { get; set; }
        public int airdropMinStartTimeSeconds { get; set; }
        public int airdropMaxStartTimeSeconds { get; set; }
        public int airdropMinOpenHeight { get; set; }
        public int airdropMaxOpenHeight { get; set; }

        public int planeMinFlyHeight { get; set; }
        public int planeMaxFlyHeight { get; set; }
        public float planeVolume { get; set; }
    }
}
