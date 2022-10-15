using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace HouseOfWisdom.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class Shapeshifter : ModNPC
    {
        public int NumberOfTimesTalkedTo = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,  // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                Direction = 1   // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
                // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
                // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // Example Person prefers the forest.
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
                                                                      //.SetBiomeAffection<ExampleSurfaceBiome>(AffectionLevel.Love) // Example Person likes the Example Surface Biome
                                                                      //.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // Loves living near the dryad.
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // Likes living near the guide.
                                                                   //.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Love) // Hates living near the demolitionist.
            ;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 100;
            NPC.defense = 500;
            NPC.lifeMax = 5000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Cool person! Change this later.")
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This code slowly rotates the NPC in the bestiary
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                drawModifiers.Rotation += 0.001f;

                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Lava);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) // Requirements for the town NPC to spawn.
        {
            for (int k = 0; k < 255; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                {
                    continue;
                }

                // Condition will be to defeat the Shapeshifter
                if (numTownNPCs >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return new ShapeshifterProfile();
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "???"
            };
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            //int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            //if (partyGirl >= 0 && Main.rand.NextBool(4))
            //{
            //    chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            //}
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add("???");

            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                //This counter is linked to a single instance of the NPC, so if the NPC is killed, the counter will reset.
                chat.Add("Maybe you'll learn my name.. someday");
            }

            return chat; // chat is implicitly cast to a string.
        }

        public override void SetChatButtons(ref string button, ref string button2) // What the chat buttons are when you open up the chat UI
        {
            button = "Shop";

            //if (Main.LocalPlayer.HasBuff(BuffID.Shine))
            //{
            //    button2 = "Travel";
            //}
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Dunno yet
        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered.
        public override bool CanGoToStatue(bool toQueenStatue) => true;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.ShadowFlame;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override void LoadData(TagCompound tag)
        {
            NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
        }
    }

    public class ShapeshifterProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("HouseOfWisdom/Content/NPCs/TownNPCs/Shapeshifter");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("HouseOfWisdom/Content/NPCs/TownNPCs/Shapeshifter_Party");

            return ModContent.Request<Texture2D>("HouseOfWisdom/Content/NPCs/TownNPCs/Shapeshifter");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("HouseOfWisdom/Content/NPCs/TownNPCs/Shapeshifter_Head");
    }
}
