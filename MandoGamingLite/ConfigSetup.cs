using BepInEx.Configuration;
using BepInEx;

namespace MandoGamingLite
{
    class ConfigSetup
    {
        public static void WriteConfig()
        {
            MGLConfig = new ConfigFile(Paths.ConfigPath + "\\MGLite_Base.cfg", true);
            EnableAltRoll = MGLConfig.Bind<bool>("1. Base", "Enable alt dive?", true, "Reassigns Tactical Dive to a new skill - this enables dive to stun nearby enemies, has slightly altered animations to match the speed/duration changes, and allows buffs to end when the skill is canceled instead of a fixed duration. May have networking issues!");
            EnableAltSlide = MGLConfig.Bind<bool>("1. Base", "Enable alt slide?", true, "Reassigns Tactical Slide to a new skill - this enables the jump height change, prevents the optional armor/invincibility from working with the mid-air jump, and allows buffs to end when the skill is canceled instead of a fixed duration. May have networking issues!");
            EnableStickyGrenade = MGLConfig.Bind<bool>("1. Base", "Enable alt grenade?", false, "Enables an unused sticky version of Frag Grenade that I've modified slightly to feel cleaner.");
            CustomMando = MGLConfig.Bind<bool>("1. Base", "Enable custom values?", false, "Creates a new config file that can be used to modify values directly. Note that skill changes listed here must be enabled to edit their values, and that the custom config file is only read IF this option is enabled. If something breaks due to this file, message me and I'll try and locate the issue.");
            DoubleSpread = MGLConfig.Bind<bool>("2. Double Tap", "Reduce spread?", true, "Reduces spread from 0.3 to 0.18. This reduces the rate at which Double Tap loses accuracy when continuously firing.");
            RoundSize = MGLConfig.Bind<bool>("3. Phase Round", "Size increase?", true, "Increases Phase Round's visuals and hitbox by 2x.");
            RoundDamage = MGLConfig.Bind<bool>("3. Phase Round", "Damage increase?", true, "Increases Phase Round's damage to 360%.");
            RoundSpeed = MGLConfig.Bind<bool>("3. Phase Round", "Speed increase?", true, "Increases Phase Round's velocity from 120 to 180.");
            RoundChain = MGLConfig.Bind<bool>("3. Phase Round", "Remove chaining?", false, "Disables the server event that allows Phase Round to increase damage on consecutive hits.");
            RoundDuration = MGLConfig.Bind<bool>("3. Phase Round", "Lower duration?", false, "Lowers the duration from 0.5s to 0.3s, allowing Commando to fire faster after using the skill.");
            BlastCopium = MGLConfig.Bind<bool>("4. Phase Blast", "Nerf damage?", true, "Reduces Phase Round damage from 8x200% to 8x185%.");
            BlastForce = MGLConfig.Bind<bool>("4. Phase Blast", "Rework force?", true, "Changes Phase Blast's second hit from 200 -> 500 force.");
            DiveSpeed = MGLConfig.Bind<bool>("5. Tactical Dive", "Speed increase?", true, "Increases the velocity of Tactical Dive. Does nothing if alt dive is enabled.");
            DiveArmor = MGLConfig.Bind<bool>("5. Tactical Dive", "Grant armor?", true, "Tactical Dive grants 100 armor temporarily.");
            DiveSprint = MGLConfig.Bind<bool>("5. Tactical Dive", "Add sprint modifier?", true, "Divides initial speed by 1.45, then multiplies by the current sprint modifier to enable Energy Drink synergy.");
            DiveInvincibility = MGLConfig.Bind<bool>("5. Tactical Dive", "Grant invincibility?", false, "Tactical Dive grants brief invincibility. Does nothing if armor is enabled.");
            DiveStock = MGLConfig.Bind<bool>("5. Tactical Dive", "Rework cooldown?", false, "Changes Dive cooldown to 3 seconds with 2 base stock, as in older versions of Mando Gaming.");
            SlideArmor = MGLConfig.Bind<bool>("6. Tactical Slide", "Grant armor?", false, "Tactical Slide grants 100 armor temporarily.");
            SlideInvincibility = MGLConfig.Bind<bool>("6. Tactical Slide", "Grant invincibility?", false, "Tactical Slide grants 100 armor temporarily.");
            SoupDamage = MGLConfig.Bind<bool>("7. Suppressive Fire", "Rebalance", true, "Increases Suppressive Fire's shot damage to 120% and bullet count to 8.");
            SoupNumber = MGLConfig.Bind<bool>("7. Suppressive Fire", "Reduce cooldown?", true, "Reduces cooldown to 8s.");
            FragRadius = MGLConfig.Bind<bool>("8. Frag Grenade", "Increase radius", true, "Increases Frag Grenade's radius to 14m.");
            FragDebuff = MGLConfig.Bind<bool>("8. Frag Grenade", "Add Shatter debuff?", false, "Frag Grenade will now inflict Shatter (-25 armor debuff) for 3 seconds on hit.");
            FragPhysics = MGLConfig.Bind<bool>("8. Frag Grenade", "Revert physics?", false, "Frag Grenades should (hopefully) act like they did pre-anniversary.");
            FragDamage = MGLConfig.Bind<bool>("8. Frag Grenade", "Damage increase?", false, "Increases Frag Grenade's base damage to 800% (200% outer).");
            FragCook = MGLConfig.Bind<bool>("8. Frag Grenade", "Cook grenades?", false, "Enables a modified version of Moffein's Cookable Frag Grenades mod.");
            FragStick = MGLConfig.Bind<bool>("9. Magnet Grenade", "Sticky grenades?", false, "Enables an unused version of Frag Grenades as an alt special, which attaches to nearby enemies and has a longer impact timer.");
            StickRadius = MGLConfig.Bind<bool>("9. Magnet Grenade", "Radius increase?", true, "Increases Magnet Grenade's radius to 14m.");
            StickDebuff = MGLConfig.Bind<bool>("9. Magnet Grenade", "Add Shatter debuff?", false, "Magnet Grenade will now inflict Shatter (-20 armor debuff) for 3 seconds on hit.");
            StickPhysics = MGLConfig.Bind<bool>("9. Magnet Grenade", "Revert physics?", false, "Magnet Grenades should (hopefully) act like they did pre-anniversary.");
            MandoGaming.MandoLog.LogInfo("Config setup complete.");
            if (CustomMando.Value)
            {
                MGCustom = new ConfigFile(Paths.ConfigPath + "\\MGLite_Custom.cfg", true);
                CDoubleSpread = MGCustom.Bind<float>("1. Double Tap", "Spread Bloom", 0.18f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CDoubleSpeed = MGCustom.Bind<float>("1. Double Tap", "Duration (frames)", 9f, "How long Double Tap delays for before firing again. Note that an extra frame is added ingame (it's only about ~0.0167s, but it adds up).");
                CDoubleDamage = MGCustom.Bind<float>("1. Double Tap", "Damage", 1f, "How much damage each shot deals.");
                CDoubleForce = MGCustom.Bind<float>("1. Double Tap", "Force", 200f, "How much outward force each shot applies.");
                CRoundDamage = MGCustom.Bind<float>("2. Phase Round", "Damage", 3.6f, "How much damage Phase Round does on its first hit.");
                CRoundForce = MGCustom.Bind<float>("2. Phase Round", "Force", 2000f, "How much force Phase Round applies.");
                CRoundProc = MGCustom.Bind<float>("2. Phase Round", "Proc Coefficient", 3f, "The proc coefficient of Phase Round.");
                CRoundSize = MGCustom.Bind<float>("2. Phase Round", "Size", 2f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CRoundSpeed = MGCustom.Bind<float>("2. Phase Round", "Velocity", 180f, "The forward speed of Phase Round.");
                CRoundDuration = MGCustom.Bind<float>("2. Phase Round", "Duration", 0.5f, "How long after Phase Round is fired before other skills are usable.");
                CBlastDamage = MGCustom.Bind<float>("3. Phase Blast", "Damage", 1.85f, "How much damage Phase Blast deals per shot.");
                CBlastCount = MGCustom.Bind<int>("3. Phase Blast", "Bullet Count", 4, "How many bullets Phase Blast fires per volley.");
                CBlastForce1 = MGCustom.Bind<float>("3. Phase Blast", "Force 1", 0f, "How much force the first volley of bullets applies.");
                CBlastForce2 = MGCustom.Bind<float>("3. Phase Blast", "Force 2", 5f, "How much force the second volley of bullets applies.");
                CBlastProc = MGCustom.Bind<float>("3. Phase Blast", "Proc Coefficient", 0.5f, "The proc coefficient of each bullet in Phase Blast.");
                CBlastDistance = MGCustom.Bind<float>("3. Phase Blast", "Distance", 25f, "How far Phase Blast bullets travel.");
                CDiveSpeed = MGCustom.Bind<float>("4. Tactical Dive", "Duration", 0.4f, "How long the dash takes to complete.");
                CDiveInitSpeed = MGCustom.Bind<float>("4. Tactical Dive", "Initial Speed", 10f, "The speed of the dash itself.");
                CDiveFnlSpeed = MGCustom.Bind<float>("4. Tactical Dive", "Final Speed", 2.25f, "The speed used when exiting Tactical Dive.");
                CDiveStun = MGCustom.Bind<float>("5. Alt Tactical Dive", "Stun Radius", 3.5f, "The radius of the brief stun applied by Alt Tactical Dive.");
                CArmorBuff = MGCustom.Bind<float>("6. Tactical Dive/Slide", "Armor Value", 100f, "Armor granted by the buff added to Tactical Dive (and optionally Slide).");
                CSlideJump = MGCustom.Bind<float>("7. Alt Tactical Slide", "Jump Modifier", 1.2f, "How much the jump height of Alt Tactical Slide is modified by.");
                CSoupDamage = MGCustom.Bind<float>("8. Suppressive Fire", "Shot Damage", 1.25f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CSoupNumber = MGCustom.Bind<int>("8. Suppressive Fire", "Shot Ammo", 8, "Value that Double Tap uses to determine how quickly spread is applied.");
                CSoupForce = MGCustom.Bind<float>("8. Suppressive Fire", "Force", 100f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CSoupShotDuration = MGCustom.Bind<float>("8. Suppressive Fire", "Shot Duration", 0.07f, "How long Suppressive Fire delays for between shots.");
                CFragDamage = MGCustom.Bind<float>("9. Frag Grenade", "Damage", 8f, "Base damage dealt by Frag Grenade; divide by 4 to get the outer radius damage.");
                CFragRadius = MGCustom.Bind<float>("9. Frag Grenade", "Radius", 14f, "Total radius of Frag Grenade; inner radius is 1/2 of this value.");
                CFragForce = MGCustom.Bind<float>("9. Frag Grenade", "Force", 1000f, "How far outward Frag Grenade knocks entities.");
                CFragUpwardForce = MGCustom.Bind<float>("9. Frag Grenade", "Upward Force", 600f, "How far upward Frag Grenade knocks entities.");
                CFragProc = MGCustom.Bind<float>("9. Frag Grenade", "Proc Coefficient", 1f, "The proc coefficient of Frag Grenade.");
                CFragSpeed = MGCustom.Bind<float>("9. Frag Grenade", "Velocity", 50f, "The forward speed of Frag Grenade.");
                CFragLifetime = MGCustom.Bind<float>("9. Frag Grenade", "Lifetime", 10f, "How long Frag Grenade lasts for normally.");
                CFragImpactTimer = MGCustom.Bind<float>("9. Frag Grenade", "Impact Lifetime", 1f, "How long Frag Grenade lasts after impact with a surface or entity.");
                CShatterDebuff = MGCustom.Bind<float>("9. Frag Grenade", "Shatter Value", 30f, "How much armor is reduced by when inflicted with Shatter.");
                CShatterTimer = MGCustom.Bind<float>("9. Frag Grenade", "Shatter Timer", 2.5f, "How long Shatter is applied for.");
                CCookChargeTimer = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Cook Timer", 0.75f, "How long Frag Grenade should be cooked for to reach its minimum impact timer.");
                CCookOverTimer = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Timer", 1.25f, "How long you can hold Frag Grenade for until it explodes.");
                CCookSelfDamage = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Cost", 0.2f, "% of health Commando loses for overcooking Frag Grenade. Note that this damage is lethal and scales off of total HP, unlike similar self-damage abilities.");
                CCookSelfForce = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Push", 4500, "How much force overcooking applies to Commando.");
                CCookOopsDamage = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Damage", 7, "How much damage overcooking deals to nearby enemies.");
                CCookOopsForce = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Force", 1000f, "How much force overcooking applies to nearby enemies.");
                CCookOopsProc = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Proc", 1f, "The proc coefficient of an overcooked grenade. Note that the damage to Commando does NOT have a proc coefficient.");
                CCookOopsRadius = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Overcook Radius", 11f, "The radius of an overcooked grenade's explosion.");
                CCookImpactMax = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Impact Maximum", 1f, "How long a Frag Grenade lasts for if thrown immediately.");
                CCookImpactMin = MGCustom.Bind<float>("ALT1. Cookable Grenade", "Impact Minimum", 0.25f, "How long a Frag Grenade lasts for if fully cooked.");
                CStickDamage = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 7f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickRadius = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 12f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickForce = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 1000f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickUpwardForce = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 600f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickProc = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 1f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickSpeed = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 50f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickLifetime = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 10f, "Value that Double Tap uses to determine how quickly spread is applied.");
                CStickImpactTimer = MGCustom.Bind<float>("ALT2. Sticky Grenade", "Spread Bloom", 2f, "Value that Double Tap uses to determine how quickly spread is applied.");
                MandoGaming.MandoLog.LogInfo("Custom config setup complete.");
            }
        }
        // Base Config
        public static ConfigFile MGLConfig { get; set; }
        public static ConfigEntry<bool> EnableAltRoll { get; set; }
        public static ConfigEntry<bool> EnableAltSlide { get; set; }
        public static ConfigEntry<bool> EnableStickyGrenade { get; set; }
        public static ConfigEntry<bool> CustomMando { get; set; }
        public static ConfigFile MGCustom { get; set; }
        // Double Tap
        public static ConfigEntry<bool> DoubleSpread { get; set; }
        public static ConfigEntry<float> CDoubleSpread { get; set; }
        public static ConfigEntry<float> CDoubleSpeed { get; set; }
        public static ConfigEntry<float> CDoubleDamage { get; set; }
        public static ConfigEntry<float> CDoubleForce { get; set; }
        // Phase Round
        public static ConfigEntry<bool> RoundSize { get; set; }
        public static ConfigEntry<bool> RoundDamage { get; set; }
        public static ConfigEntry<bool> RoundSpeed { get; set; }
        public static ConfigEntry<bool> RoundChain { get; set; }
        public static ConfigEntry<bool> RoundDuration { get; set; }
        public static ConfigEntry<float> CRoundSize { get; set; }
        public static ConfigEntry<float> CRoundDamage { get; set; }
        public static ConfigEntry<float> CRoundSpeed { get; set; }
        public static ConfigEntry<float> CRoundDuration { get; set; }
        public static ConfigEntry<float> CRoundForce { get; set; }
        public static ConfigEntry<float> CRoundProc { get; set; }
        // Phase Blast
        public static ConfigEntry<bool> BlastCopium { get; set; }
        public static ConfigEntry<bool> BlastForce { get; set; }
        public static ConfigEntry<float> CBlastDamage { get; set; }
        public static ConfigEntry<float> CBlastForce1 { get; set; }
        public static ConfigEntry<float> CBlastForce2 { get; set; }
        public static ConfigEntry<int> CBlastCount { get; set; }
        public static ConfigEntry<float> CBlastDistance { get; set; }
        public static ConfigEntry<float> CBlastProc { get; set; }
        // Tactical Dive
        public static ConfigEntry<bool> DiveSpeed { get; set; }
        public static ConfigEntry<bool> DiveArmor { get; set; }
        public static ConfigEntry<bool> DiveInvincibility { get; set; }
        public static ConfigEntry<bool> DiveStock { get; set; }
        public static ConfigEntry<bool> DiveSprint { get; set; }
        public static ConfigEntry<float> CDiveSpeed { get; set; }
        public static ConfigEntry<float> CDiveInitSpeed { get; set; }
        public static ConfigEntry<float> CDiveFnlSpeed { get; set; }
        public static ConfigEntry<float> CArmorBuff { get; set; }
        public static ConfigEntry<float> CDiveStun { get; set; }
        // Tactical Slide
        public static ConfigEntry<bool> SlideArmor { get; set; }
        public static ConfigEntry<bool> SlideInvincibility { get; set; }
        public static ConfigEntry<float> CSlideJump { get; set; }
        // Soup Fire
        public static ConfigEntry<bool> SoupDamage { get; set; }
        public static ConfigEntry<bool> SoupNumber { get; set; }
        public static ConfigEntry<float> CSoupDamage { get; set; }
        public static ConfigEntry<int> CSoupNumber { get; set; }
        public static ConfigEntry<float> CSoupShotDuration { get; set; }
        public static ConfigEntry<float> CSoupForce { get; set; }
        // Frag Grenade - 8/14
        public static ConfigEntry<bool> FragRadius { get; set; }
        public static ConfigEntry<bool> FragDebuff { get; set; }
        public static ConfigEntry<bool> FragPhysics { get; set; }
        public static ConfigEntry<bool> FragDamage { get; set; }
        public static ConfigEntry<bool> FragCook { get; set; }
        public static ConfigEntry<bool> FragStick { get; set; }
        public static ConfigEntry<bool> StickShatter { get; set; }
        public static ConfigEntry<bool> StickRadius { get; set; }
        public static ConfigEntry<bool> StickDebuff { get; set; }
        public static ConfigEntry<bool> StickPhysics { get; set; }
        public static ConfigEntry<float> CFragRadius { get; set; }
        public static ConfigEntry<float> CShatterDebuff { get; set; }
        public static ConfigEntry<float> CShatterTimer { get; set; }
        public static ConfigEntry<float> CFragDamage { get; set; }
        public static ConfigEntry<float> CFragLifetime { get; set; }
        public static ConfigEntry<float> CFragSpeed { get; set; }
        public static ConfigEntry<float> CFragImpactTimer { get; set; }
        public static ConfigEntry<float> CFragProc { get; set; }
        public static ConfigEntry<float> CFragForce { get; set; }
        public static ConfigEntry<float> CFragUpwardForce { get; set; }
        // Cook Grenade 0/10
        public static ConfigEntry<float> CCookChargeTimer { get; set; } // ?
        public static ConfigEntry<float> CCookOverTimer { get; set; } // ?
        public static ConfigEntry<float> CCookSelfDamage { get; set; } // ?
        public static ConfigEntry<float> CCookSelfForce { get; set; } // ?
        public static ConfigEntry<float> CCookOopsDamage { get; set; } // ?
        public static ConfigEntry<float> CCookOopsRadius { get; set; } // ?
        public static ConfigEntry<float> CCookOopsForce { get; set; } // ?
        public static ConfigEntry<float> CCookOopsProc { get; set; } // ?
        public static ConfigEntry<float> CCookImpactMax { get; set; } // ?
        public static ConfigEntry<float> CCookImpactMin { get; set; } // ?
        // Sticky Grenade
        public static ConfigEntry<float> CStickRadius { get; set; }
        public static ConfigEntry<float> CStickDamage { get; set; }
        public static ConfigEntry<float> CStickLifetime { get; set; }
        public static ConfigEntry<float> CStickSpeed { get; set; }
        public static ConfigEntry<float> CStickImpactTimer { get; set; }
        public static ConfigEntry<float> CStickProc { get; set; }
        public static ConfigEntry<float> CStickForce { get; set; }
        public static ConfigEntry<float> CStickUpwardForce { get; set; }
    }
}
