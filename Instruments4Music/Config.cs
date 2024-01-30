using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instruments4Music
{
    public class Config
    {
        public static void Load()
        {
            Config.configActiveHorn = Instruments4MusicPlugin.config.Bind("InstrumentsActive", "ActiveHorn", true, "Active \"Horn\" as an instrument.");
        }

        public static ConfigEntry<bool> configActiveHorn;
    }
}
