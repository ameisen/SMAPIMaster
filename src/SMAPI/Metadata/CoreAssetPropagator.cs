using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Framework.Reflection;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Projectiles;
using StardewValley.TerrainFeatures;

namespace StardewModdingAPI.Metadata
{
    /// <summary>Propagates changes to core assets to the game state.</summary>
    internal class CoreAssetPropagator
    {
        /*********
        ** Properties
        *********/
        /// <summary>Normalises an asset key to match the cache key.</summary>
        private readonly Func<string, string> GetNormalisedPath;

        /// <summary>Simplifies access to private game code.</summary>
        private readonly Reflector Reflection;


        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the core asset data.</summary>
        /// <param name="getNormalisedPath">Normalises an asset key to match the cache key.</param>
        /// <param name="reflection">Simplifies access to private code.</param>
        public CoreAssetPropagator(Func<string, string> getNormalisedPath, Reflector reflection)
        {
            this.GetNormalisedPath = getNormalisedPath;
            this.Reflection = reflection;
        }

        /// <summary>Reload one of the game's core assets (if applicable).</summary>
        /// <param name="content">The content manager through which to reload the asset.</param>
        /// <param name="key">The asset key to reload.</param>
        /// <returns>Returns whether an asset was reloaded.</returns>
        public bool Propagate(LocalizedContentManager content, string key)
        {
            return this.PropagateImpl(content, key) != null;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Reload one of the game's core assets (if applicable).</summary>
        /// <param name="content">The content manager through which to reload the asset.</param>
        /// <param name="key">The asset key to reload.</param>
        /// <returns>Returns any non-null value to indicate an asset was loaded..</returns>
        private object PropagateImpl(LocalizedContentManager content, string key)
        {
            Reflector reflection = this.Reflection;
            switch (key.ToLower().Replace("/", "\\")) // normalised key so we can compare statically
            {
                /****
                ** Buildings
                ****/
                case "buildings\\houses": // Farm
#if STARDEW_VALLEY_1_3
                    reflection.GetField<Texture2D>(typeof(Farm), nameof(Farm.houseTextures)).SetValue(content.Load<Texture2D>(key));
                    return true;
#else
                    {
                        Farm farm = Game1.getFarm();
                        if (farm == null)
                            return null;
                        return farm.houseTextures = content.Load<Texture2D>(key);
                    }
#endif

                /****
                ** Content\Characters\Farmer
                ****/
                case "characters\\farmer\\accessories": // Game1.loadContent
                    return FarmerRenderer.accessoriesTexture = content.Load<Texture2D>(key);

                case "characters\\farmer\\farmer_base": // Farmer
                    if (Game1.player == null || !Game1.player.isMale)
                        return null;
#if STARDEW_VALLEY_1_3
                    return Game1.player.FarmerRenderer = new FarmerRenderer(key);
#else
                    return Game1.player.FarmerRenderer = new FarmerRenderer(content.Load<Texture2D>(key));
#endif

                case "characters\\farmer\\farmer_girl_base": // Farmer
                    if (Game1.player == null || Game1.player.isMale)
                        return null;
#if STARDEW_VALLEY_1_3
                    return Game1.player.FarmerRenderer = new FarmerRenderer(key);
#else
                    return Game1.player.FarmerRenderer = new FarmerRenderer(content.Load<Texture2D>(key));
#endif

                case "characters\\farmer\\hairstyles": // Game1.loadContent
                    return FarmerRenderer.hairStylesTexture = content.Load<Texture2D>(key);

                case "characters\\farmer\\hats": // Game1.loadContent
                    return FarmerRenderer.hatsTexture = content.Load<Texture2D>(key);

                case "characters\\farmer\\shirts": // Game1.loadContent
                    return FarmerRenderer.shirtsTexture = content.Load<Texture2D>(key);

                /****
                ** Content\Data
                ****/
                case "data\\achievements": // Game1.loadContent
                    return Game1.achievements = content.Load<Dictionary<int, string>>(key);

                case "data\\bigcraftablesinformation": // Game1.loadContent
                    return Game1.bigCraftablesInformation = content.Load<Dictionary<int, string>>(key);

                case "data\\cookingrecipes": // CraftingRecipe.InitShared
                    return CraftingRecipe.cookingRecipes = content.Load<Dictionary<string, string>>(key);

                case "data\\craftingrecipes": // CraftingRecipe.InitShared
                    return CraftingRecipe.craftingRecipes = content.Load<Dictionary<string, string>>(key);

                case "data\\npcgifttastes": // Game1.loadContent
                    return Game1.NPCGiftTastes = content.Load<Dictionary<string, string>>(key);

                case "data\\objectinformation": // Game1.loadContent
                    return Game1.objectInformation = content.Load<Dictionary<int, string>>(key);

                /****
                ** Content\Fonts
                ****/
                case "fonts\\spritefont1": // Game1.loadContent
                    return Game1.dialogueFont = content.Load<SpriteFont>(key);

                case "fonts\\smallfont": // Game1.loadContent
                    return Game1.smallFont = content.Load<SpriteFont>(key);

                case "fonts\\tinyfont": // Game1.loadContent
                    return Game1.tinyFont = content.Load<SpriteFont>(key);

                case "fonts\\tinyfontborder": // Game1.loadContent
                    return Game1.tinyFontBorder = content.Load<SpriteFont>(key);

                /****
                ** Content\Lighting
                ****/
                case "loosesprites\\lighting\\greenlight": // Game1.loadContent
                    return Game1.cauldronLight = content.Load<Texture2D>(key);

                case "loosesprites\\lighting\\indoorwindowlight": // Game1.loadContent
                    return Game1.indoorWindowLight = content.Load<Texture2D>(key);

                case "loosesprites\\lighting\\lantern": // Game1.loadContent
                    return Game1.lantern = content.Load<Texture2D>(key);

                case "loosesprites\\lighting\\sconcelight": // Game1.loadContent
                    return Game1.sconceLight = content.Load<Texture2D>(key);

                case "loosesprites\\lighting\\windowlight": // Game1.loadContent
                    return Game1.windowLight = content.Load<Texture2D>(key);

                /****
                ** Content\LooseSprites
                ****/
                case "loosesprites\\controllermaps": // Game1.loadContent
                    return Game1.controllerMaps = content.Load<Texture2D>(key);

                case "loosesprites\\cursors": // Game1.loadContent
                    return Game1.mouseCursors = content.Load<Texture2D>(key);

                case "loosesprites\\daybg": // Game1.loadContent
                    return Game1.daybg = content.Load<Texture2D>(key);

                case "loosesprites\\font_bold": // Game1.loadContent
                    return SpriteText.spriteTexture = content.Load<Texture2D>(key);

                case "loosesprites\\font_colored": // Game1.loadContent
                    return SpriteText.coloredTexture = content.Load<Texture2D>(key);

                case "loosesprites\\nightbg": // Game1.loadContent
                    return Game1.nightbg = content.Load<Texture2D>(key);

                case "loosesprites\\shadow": // Game1.loadContent
                    return Game1.shadowTexture = content.Load<Texture2D>(key);

                /****
                ** Content\Critters
                ****/
                case "tilesheets\\critters": // Criter.InitShared
                    return Critter.critterTexture = content.Load<Texture2D>(key);

                case "tilesheets\\crops": // Game1.loadContent
                    return Game1.cropSpriteSheet = content.Load<Texture2D>(key);

                case "tilesheets\\debris": // Game1.loadContent
                    return Game1.debrisSpriteSheet = content.Load<Texture2D>(key);

                case "tilesheets\\emotes": // Game1.loadContent
                    return Game1.emoteSpriteSheet = content.Load<Texture2D>(key);

                case "tilesheets\\furniture": // Game1.loadContent
                    return Furniture.furnitureTexture = content.Load<Texture2D>(key);

                case "tilesheets\\projectiles": // Game1.loadContent
                    return Projectile.projectileSheet = content.Load<Texture2D>(key);

                case "tilesheets\\rain": // Game1.loadContent
                    return Game1.rainTexture = content.Load<Texture2D>(key);

                case "tilesheets\\tools": // Game1.ResetToolSpriteSheet
                    Game1.ResetToolSpriteSheet();
                    return true;

                case "tilesheets\\weapons": // Game1.loadContent
                    return Tool.weaponsTexture = content.Load<Texture2D>(key);

                /****
                ** Content\Maps
                ****/
                case "maps\\menutiles": // Game1.loadContent
                    return Game1.menuTexture = content.Load<Texture2D>(key);

                case "maps\\springobjects": // Game1.loadContent
                    return Game1.objectSpriteSheet = content.Load<Texture2D>(key);

                case "maps\\walls_and_floors": // Wallpaper
                    return Wallpaper.wallpaperTexture = content.Load<Texture2D>(key);

                /****
                ** Content\Minigames
                ****/
                case "minigames\\clouds": // TitleMenu
                    if (Game1.activeClickableMenu is TitleMenu)
                    {
                        reflection.GetField<Texture2D>(Game1.activeClickableMenu, "cloudsTexture").SetValue(content.Load<Texture2D>(key));
                        return true;
                    }

                    return null;

                case "minigames\\titlebuttons": // TitleMenu
                    if (Game1.activeClickableMenu is TitleMenu titleMenu)
                    {
                        Texture2D texture = content.Load<Texture2D>(key);
                        reflection.GetField<Texture2D>(titleMenu, "titleButtonsTexture").SetValue(texture);
                        foreach (TemporaryAnimatedSprite bird in reflection.GetField<List<TemporaryAnimatedSprite>>(titleMenu, "birds").GetValue())
#if STARDEW_VALLEY_1_3
                            bird.texture = texture;
#else
                            bird.Texture = texture;
#endif
                        return true;
                    }

                    return null;

                /****
                ** Content\TileSheets
                ****/
                case "tilesheets\\animations": // Game1.loadContent
                    return Game1.animations = content.Load<Texture2D>(key);

                case "tilesheets\\buffsicons": // Game1.loadContent
                    return Game1.buffsIcons = content.Load<Texture2D>(key);

                case "tilesheets\\bushes": // new Bush()
#if STARDEW_VALLEY_1_3
                    reflection.GetField<Lazy<Texture2D>>(typeof(Bush), "texture").SetValue(new Lazy<Texture2D>(() => content.Load<Texture2D>(key)));
                    return true;
#else
                    return Bush.texture = content.Load<Texture2D>(key);
#endif

                case "tilesheets\\craftables": // Game1.loadContent
                    return Game1.bigCraftableSpriteSheet = content.Load<Texture2D>(key);

                case "tilesheets\\fruittrees": // FruitTree
                    return FruitTree.texture = content.Load<Texture2D>(key);

                /****
                ** Content\TerrainFeatures
                ****/
                case "terrainfeatures\\flooring": // Flooring
                    return Flooring.floorsTexture = content.Load<Texture2D>(key);

                case "terrainfeatures\\hoedirt": // from HoeDirt
                    return HoeDirt.lightTexture = content.Load<Texture2D>(key);

                case "Terrainfeatures\\hoedirtdark": // from HoeDirt
                    return HoeDirt.darkTexture = content.Load<Texture2D>(key);

                case "Terrainfeatures\\hoedirtsnow": // from HoeDirt
                    return HoeDirt.snowTexture = content.Load<Texture2D>(key);
            }

            // building textures
            if (key.StartsWith(this.GetNormalisedPath("Buildings\\"), StringComparison.InvariantCultureIgnoreCase))
            {
                Building[] buildings = this.GetAllBuildings().Where(p => key.Equals(this.GetNormalisedPath($"Buildings\\{p.buildingType?.ToLower()}"), StringComparison.InvariantCultureIgnoreCase)).ToArray();
                if (buildings.Any())
                {
#if STARDEW_VALLEY_1_3
                    foreach (Building building in buildings)
                        building.texture = new Lazy<Texture2D>(() => content.Load<Texture2D>(key));
#else
                    Texture2D texture = content.Load<Texture2D>(key);
                    foreach (Building building in buildings)
                        building.texture = texture;
#endif

                    return true;
                }
                return null;
            }

            return null;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get all player-constructed buildings in the world.</summary>
        private IEnumerable<Building> GetAllBuildings()
        {
            foreach (BuildableGameLocation location in Game1.locations.OfType<BuildableGameLocation>())
            {
                foreach (Building building in location.buildings)
                    yield return building;
            }
        }
    }
}
