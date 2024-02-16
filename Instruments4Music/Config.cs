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
            configActiveHorn = Instruments4MusicPlugin.config?.Bind("InstrumentsActive", "ActiveHorn", true, "Active \"Horn\" as an instrument.");
            configHUDScale = Instruments4MusicPlugin.config?.Bind("InstrumentsHUD", "HUDScale", 1.0f, "Scale the music HUD.");
        }

        public static ConfigEntry<bool>? configActiveHorn;
        public static ConfigEntry<float>? configHUDScale;
    }
}
