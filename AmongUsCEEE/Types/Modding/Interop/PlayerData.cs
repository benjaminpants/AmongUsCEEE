using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using UnityEngine;

namespace AmongUsCEEE
{
    [MoonSharpUserData]
    public struct PlayerCosmetic //container for player cosmetics
    {
        [MoonSharpVisible(false)]
        private GameData.PlayerOutfit outfit;

        public PlayerCosmetic(GameData.PlayerOutfit fit)
        {
            outfit = fit;
        }

        [MoonSharpVisible(true)]
        public string Name { get => outfit.PlayerName; }

        [MoonSharpVisible(true)]
        public string HatId { get => outfit.HatId; }

        [MoonSharpVisible(true)]
        public string PetId { get => outfit.PetId; }

        [MoonSharpVisible(true)]
        public string SkinId { get => outfit.SkinId; }

        [MoonSharpVisible(true)]
        public string VisorId { get => outfit.VisorId; }

        [MoonSharpVisible(true)]
        public string NamePlateId { get => outfit.NamePlateId; }

    }

    public static class DataExtension
    {
        public static PlayerInfoData GetInteropData(this GameData.PlayerInfo pf)
        {
            return new PlayerInfoData(pf);
        }
    }

    public struct PlayerInfoData
    {
        private GameData.PlayerInfo pi;

        [MoonSharpVisible(true)]
        public byte PlayerId { get => pi.PlayerId; }

        public Dictionary<PlayerOutfitType, PlayerCosmetic> Outfits = new Dictionary<PlayerOutfitType, PlayerCosmetic>();

        public PlayerInfoData(GameData.PlayerInfo c) 
        {
            pi = c;
            foreach (Il2CppSystem.Collections.Generic.KeyValuePair<PlayerOutfitType, GameData.PlayerOutfit> kvp in c.Outfits)
            {
                Outfits.Add(kvp.key,new PlayerCosmetic(kvp.value));
            }
        }

        public static explicit operator PlayerInfoData(GameData.PlayerInfo pf) => new PlayerInfoData(pf);
    }
}
