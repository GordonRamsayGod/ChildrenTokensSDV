using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChildrenTokens
{
    public interface IContentPatcherAPI
    {
        /// <summary>Register a simple token.</summary>
        /// <param name="mod">The manifest of the mod defining the token (see <see cref="Mod.ModManifest"/> on your entry class).</param>
        /// <param name="name">The token name. This only needs to be unique for your mod; Content Patcher will prefix it with your mod ID automatically, like <c>YourName.ExampleMod/SomeTokenName</c>.</param>
        /// <param name="getValue">A function which returns the current token value. If this returns a null or empty list, the token is considered unavailable in the current context and any patches or dynamic tokens using it are disabled.</param>
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);
    }

    public class ChildData
    {
        public int Gender { get; set; } = 0;
        public string HeShe { get; set; } = "He";
        public string HimHer { get; set; } = "Him";
        public string HisHers { get; set; } = "His";
        public string Self { get; set; } = "Himself";
        public string BoyGirl { get; set; } = "Boy";
    }

    public class ModEntry : Mod
    {
        private ChildData[] Data = new ChildData[0];
        private bool FirstInvalidate;
        private int LastRefreshTick;
        public override void Entry(IModHelper helper)
        {
            
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.SaveLoaded += DayStarted;
            
            
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Set our Content Patcher tokens
            RegisterTokens();
        }

        private void DayStarted(object sender, SaveLoadedEventArgs e)
        {
            // Set child data for male or female definitions at the start of each day
            RefreshChildData();
            if (!FirstInvalidate)
            {
                FirstInvalidate = true;
                const string path = @"Characters/Dialogue/";
                
                Helper.Content.InvalidateCache(path + "MarriageDialogue");
                foreach (string npc in Game1.NPCGiftTastes.Keys)
                {
                    Helper.Content.InvalidateCache(path + npc);
                    Helper.Content.InvalidateCache(path + "/MarriageDialogue" + npc);
                }
            }
        }
        private IEnumerable<Child> GetChildren()
        {
            // get farmhouse
            FarmHouse farmhouse = Context.IsWorldReady
                ? (FarmHouse)Game1.getLocationFromName("FarmHouse")
                : SaveGame.loaded?.locations?.OfType<FarmHouse>().FirstOrDefault(p => p.Name == "FarmHouse");

            // get children
            if (farmhouse != null)
            {
                foreach (Child child in farmhouse.characters.OfType<Child>())
                    yield return child;
            }
        }
        private void RefreshChildData()
        {
            if (Game1.ticks <= this.LastRefreshTick)
                return;
            this.LastRefreshTick = Game1.ticks;

            // Set the values for all children, defaulting to male
            if (SaveGame.loaded != null)
            {
                FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);
                this.Data = this.GetChildren().Select(child =>
                {
                    var cData = new ChildData();

                    if (child.Gender == 1)
                    {
                        cData.BoyGirl = "Girl";
                        cData.HeShe = "She";
                        cData.HimHer = "Her";
                        cData.HisHers = "Hers";
                        cData.Self = "Herself";
                    }

                    return cData;
                }).ToArray();

            }
            
           
        }

        private void RegisterTokens()
        {
            var api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(ModManifest, "FirstChildGender", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?new[] { Data.Length > 0 ? Data[0].Gender.ToString() : null }
                :new string[0];
            });
            api.RegisterToken(ModManifest, "SecondChildGender", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ? 
                new[] { Data.Length > 1 ? Data[1].Gender.ToString() : null }
                : new string[0];
            });
            api.RegisterToken(ModManifest, "HeShe", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?new[] { Data.Length > 0 ? Data[0].HeShe : null }
                 : new string[0];
            });
            api.RegisterToken(ModManifest, "HeSheSec", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?new[] { Data.Length > 1 ? Data[1].HeShe : null }
                 : new string[0];
            });

            api.RegisterToken(ModManifest, "HimHer", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?new[] { Data.Length > 0 ? Data[0].HimHer : null }
                 : new string[0];
            });
            api.RegisterToken(ModManifest, "HimHerSec", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?
                new[] { Data.Length > 1 ? Data[1].HimHer : null }
                : new string[0];
            });

            api.RegisterToken(ModManifest, "HisHers", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?
                new[] { Data.Length > 0 ? Data[0].HisHers : null }
                : new string[0];
            });
            api.RegisterToken(ModManifest, "HisHersSec", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?
                new[] { Data.Length > 1 ? Data[1].HisHers : null }
                 : new string[0];
            });

            api.RegisterToken(ModManifest, "Self", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?
                new[] { Data.Length > 0 ? Data[0].Self : null }
                : new string[0];
            });
            api.RegisterToken(ModManifest, "SelfSec", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ?
                new[] { Data.Length > 1 ? Data[1].Self : null }
                : new string[0];
            });

            api.RegisterToken(ModManifest, "BoyGirl", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ? new[] { Data.Length > 0 ? Data[0].BoyGirl : null }
                : new string[0];
            });
            api.RegisterToken(ModManifest, "BoyGirlSec", () =>
            {
                this.RefreshChildData();
                return Data.Length > 0
                ? new[] { Data.Length > 1 ? Data[1].BoyGirl : null }
                : new string[0];
            });
            foreach (string npc in Game1.NPCGiftTastes.Keys)
            {
                Helper.Content.InvalidateCache(@"Characters/Dialogue/" + npc);
            }
            
        }
    }
}