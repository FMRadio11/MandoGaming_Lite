using RoR2;
using UnityEngine;
using R2API;
using System;
using R2API.Utils;
using EntityStates.Commando.CommandoWeapon;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates;

namespace MandoGamingLite
{
    class MandoGaming : ConfigSetup
    {
        public static void MGLite()
        {
            ConfigSetup.WriteConfig();
            BoolSetup();
            if (DiveArmor.Value || SlideArmor.Value || FragDebuff.Value) BuffSetup();
            On.RoR2.BodyCatalog.Init += orig =>
            {
                orig();
                MandoGaming.MandoLog.LogInfo("Initializing BodyCatalog hook...");
                mandoPrefab = BodyCatalog.FindBodyPrefab("CommandoBody");
                SkillLocator mandoSL = mandoPrefab.GetComponent<SkillLocator>();
                SkillEdits(mandoSL);
                Skills(mandoSL);
            };
            GrenadeSetup();
        }
        public static void SkillEdits(SkillLocator mandoSL)
        {
            if (DoubleEdit)
            {
                // Double Tap isn't changed much - mostly just accounting for custom config stuff. There is, however, a small reduction in spread bloom - it'll take more time to lose accuracy when repeatedly fired.
                On.EntityStates.Commando.CommandoWeapon.FirePistol2.OnEnter += (orig, self) =>
                {
                    if (DoubleSpread.Value) FirePistol2.spreadBloomValue = CustomMando.Value ? CDoubleSpread.Value : 0.18f;
                    if (CustomMando.Value)
                    {
                        FirePistol2.baseDuration = CDoubleSpeed.Value / 60;
                        FirePistol2.damageCoefficient = CDoubleDamage.Value;
                        FirePistol2.force = CDoubleForce.Value;
                    }
                    orig(self);
                };
                if (CustomMando.Value && CDoubleDamage.Value != 1) mandoSL.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "Rapidly shoot an enemy for <style=cIsDamage>" + CDoubleDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "") + " damage</style>.";
            }
            if (RoundEdit)
            {
                // Loading projectile information...
                SkillDef RoundDef = mandoSL.secondary.skillFamily.variants[0].skillDef;
                GameObject roundObject = Resources.Load<GameObject>("prefabs/projectiles/fmjramping");
                GameObject roundGhost = Resources.Load<GameObject>("prefabs/projectileghosts/fmjrampingghost");
                ProjectileOverlapAttack roundDamage = roundObject.GetComponent<ProjectileOverlapAttack>();
                ProjectileSimple roundProj = roundObject.GetComponent<ProjectileSimple>();
                // A new 2x size vector is applied to both objects' localScale.
                if (RoundSize.Value)
                {
                    Vector3 sizeVector = new Vector3(2f, 2f, 2f);
                    if (CustomMando.Value) sizeVector = new Vector3(CRoundSize.Value, CRoundSize.Value, CRoundSize.Value);
                    roundObject.transform.localScale = sizeVector;
                    roundGhost.transform.localScale = sizeVector;
                }
                // More generic changes here
                if (RoundSpeed.Value) roundProj.desiredForwardSpeed = ((CustomMando.Value) ? CRoundSpeed.Value : 180);
                String desc = (!RoundChain.Value ? "<style=cIsDamage>Ramping</style>. " : "");
                if (RoundChain.Value) roundDamage.onServerHit = null;
                desc = desc + "Fire a a <style=cIsDamage>piercing</style> bullet for <style=cIsDamage>";
                if (CustomMando.Value && CRoundDamage.Value != 3) desc += CRoundDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "");
                else desc = desc + (RoundDamage.Value ? "360%" : "300%");
                desc = desc + " damage</style>. Goes through terrain.";
                if (CustomMando.Value) roundDamage.overlapProcCoefficient = CRoundProc.Value;
                // The chaining damage is altered into a keyword to enable reverting the description back to its original state.
                LanguageAPI.Add("KEYWORD_RAMPING", "<style=cKeywordName>Ramping</style><style=cSub>This skill gains <style=cIsDamage>1.4x damage</style> for every enemy it passes through.</style>");
                if (!RoundChain.Value) RoundDef.keywordTokens = new string[] { "KEYWORD_RAMPING" };
                RoundDef.skillDescriptionToken = desc;
            }
            if (BlastEdit)
            {
                // Nerfing damage. Force is called twice - once before OnEnter in order to affect the first volley, and once after for the second volley (since the first is fired during OnEnter code).
                On.EntityStates.Commando.CommandoWeapon.FireShotgunBlast.OnEnter += (orig, self) =>
                {
                    if (BlastCopium.Value) self.damageCoefficient = (CustomMando.Value ? CBlastDamage.Value : 1.85f);
                    if (BlastForce.Value) self.force = (CustomMando.Value ? CBlastForce1.Value : 200);
                    if (CustomMando.Value)
                    {
                        self.procCoefficient = CBlastProc.Value;
                        self.maxDistance = CBlastDistance.Value;
                        self.bulletCount = CBlastCount.Value;
                    }
                    orig(self);
                    if (BlastForce.Value) self.force = (CustomMando.Value ? CBlastForce2.Value : 600f);
                    if (CustomMando.Value)
                    {

                    }
                };
                SkillDef BlastDef = mandoSL.secondary.skillFamily.variants[1].skillDef;
                bool CBlastCheck = false;
                if (CustomMando.Value) CBlastCheck = CBlastCount.Value != 4 || CBlastDamage.Value != 2;
                string BlastDesc = "<style=cIsDamage>Stunning</style>. Fire two blasts that deal <style=cIsDamage>";
                if (CustomMando.Value && CBlastCheck) BlastDesc += CBlastCount.Value.ToString() + "x" + CBlastDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "");
                else BlastDesc += "8x" + ((BlastCopium.Value) ? "185%" : "200%");
                BlastDesc += " damage</style> total.";
                if (BlastForce.Value) BlastDesc += " The second blast has <style=cIsDamage>additional knockback</style>."; 
                BlastDef.skillDescriptionToken = BlastDesc;
            }
            if (DiveEdit)
            {
                // Modifies
                On.EntityStates.Commando.DodgeState.OnEnter += (orig, self) =>
                {
                    if (DiveArmor.Value) self.outer.commonComponents.characterBody.AddTimedBuff(rollArmor, 0.45f);
                    else if (DiveInvincibility.Value) self.outer.commonComponents.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.45f);
                    if (DiveSpeed.Value || DiveSprint.Value)
                    {
                        if (DiveSprint.Value && DiveSpeed.Value) self.initialSpeedCoefficient = ((CustomMando.Value ? CDiveInitSpeed.Value : 10) / 1.45f) * self.outer.commonComponents.characterBody.sprintingSpeedMultiplier;
                        else if (DiveSpeed.Value) self.initialSpeedCoefficient = CustomMando.Value ? CDiveInitSpeed.Value : 10;
                        else self.initialSpeedCoefficient = (self.initialSpeedCoefficient / 1.45f) * self.outer.commonComponents.characterBody.sprintingSpeedMultiplier;
                        self.finalSpeedCoefficient = CustomMando.Value ? CDiveFnlSpeed.Value : 2.25f;
                        if (CustomMando.Value) self.duration = CDiveSpeed.Value;
                    }
                    orig(self);
                };
            }
            SkillDef DiveDef = mandoSL.utility.skillFamily.variants[0].skillDef;
            String rollDef = "Quickly roll in the direction held";
            if (DiveArmor.Value || DiveInvincibility.Value)
            {
                rollDef += ", gaining <style=cIsUtility>";
                if (DiveInvincibility.Value) rollDef += "invincibility";
                else rollDef += "armor";
                rollDef += "</style> for the duration";
                if (EnableAltRoll.Value) rollDef += " and <style=cIsDamage>stunning</style> nearby enemies";
            }
            else if (EnableAltRoll.Value) rollDef += ", <style=cIsDamage>stunning</style> nearby enemies";
            rollDef = rollDef + ".";
            if (DiveStock.Value)
            {
                DiveDef.baseMaxStock = 2;
                DiveDef.baseRechargeInterval = 3;
                rollDef += " Hold up to 2.";
            }
            DiveDef.skillDescriptionToken = rollDef;
            if (SlideEdit)
            {
                On.EntityStates.Commando.SlideState.OnEnter += (orig, self) =>
                {
                    if (SlideArmor.Value) self.outer.commonComponents.characterBody.AddTimedBuff(rollArmor, 0.45f);
                    else if (SlideInvincibility.Value) self.outer.commonComponents.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.45f);
                    orig(self);
                };
            }
            SkillDef SlideDef = mandoSL.utility.skillFamily.variants[1].skillDef;
            String slideDesc = "<style=cIsUtility>Slide</style> on the ground for a short distance";
            if (SlideArmor.Value || SlideInvincibility.Value)
            {
                slideDesc += "while gaining <style=cIsUtility>";
                if (SlideInvincibility.Value) slideDesc += "invincibility";
                else slideDesc += "armor";
                slideDesc += "</style>";
            }
            slideDesc += ", or <style=cIsUtility>jump again</style> in the air. You can <style=cIsDamage>use other skills while sliding</style>.";
            SlideDef.skillDescriptionToken = slideDesc;
            if (SoupEdit)
            {
                if (SoupDamage.Value) On.EntityStates.Commando.CommandoWeapon.FireBarrage.OnEnter += (orig, self) =>
                {
                    FireBarrage.baseBulletCount = CustomMando.Value ? CSoupNumber.Value : 8;
                    FireBarrage.baseDurationBetweenShots = CustomMando.Value ? CSoupShotDuration.Value : 0.07f;
                    FireBarrage.damageCoefficient = CustomMando.Value ? CSoupDamage.Value : 1.25f;
                    if (CustomMando.Value) FireBarrage.force = CSoupForce.Value;
                    orig(self);
                };
                SkillDef SoupDef = mandoSL.special.skillFamily.variants[0].skillDef;
                if (SoupNumber.Value) SoupDef.baseRechargeInterval = 8;
                if (SoupDamage.Value || CustomMando.Value)
                {
                    String soupDesc = "Fire rapidly for <style=cIsDamage>";
                    if (CustomMando.Value) soupDesc += CSoupNumber.Value + "x" + CSoupDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "");
                    else soupDesc += "8x125%";
                    soupDesc += "</style> damage; increased attack speed adds more shots.";
                    SoupDef.skillDescriptionToken = soupDesc;
                }
            }
            if (FragEdit)
            {
                SkillDef FragDef = mandoSL.special.skillFamily.variants[1].skillDef;
                String fragDesc = "";
                if (FragDebuff.Value)
                {
                    String shatterDesc = "<style=cKeywordName>Shattering</style><style=cSub><style=cIsDamage>Reduces armor by -" + (CustomMando.Value? CShatterDebuff.Value.ToString() : "20") + "</style> for " + (CustomMando.Value ? CShatterTimer.Value.ToString() : "2.5") + " seconds.</style>";
                    LanguageAPI.Add("KEYWORD_SHATTERING", shatterDesc);
                    fragDesc = fragDesc + "<style=cIsDamage>Shattering</style>. ";
                    FragDef.keywordTokens = new string[] { "KEYWORD_SHATTERING" };
                }
                fragDesc += "Throw a grenade that explodes for <style=cIsDamage>";
                if (CustomMando.Value && FragDamage.Value) fragDesc += CFragDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "");
                else if (FragDamage.Value) fragDesc += "800%";
                else fragDesc += "700%";
                fragDesc += " damage</style>. Can hold up to 2.";
                FragDef.skillDescriptionToken = fragDesc;
            }
            if (FragEdit || RoundEdit)
            {
                On.EntityStates.GenericProjectileBaseState.OnEnter += (orig, self) =>
                {
                    if (self is ThrowGrenade)
                    {
                        if (FragDamage.Value) self.damageCoefficient = CustomMando.Value ? CFragDamage.Value : 8;
                        if (CustomMando.Value) self.force = CFragForce.Value;
                    }
                    if (self is ThrowStickyGrenade && CustomMando.Value)
                    {
                        self.force = CFragForce.Value;
                        self.damageCoefficient = CStickDamage.Value;
                    }
                    if (self is FireFMJ && !(self is ThrowGrenade))
                    {
                        if (RoundDamage.Value) self.damageCoefficient = CustomMando.Value ? CRoundDamage.Value : 3.6f;
                        if (RoundDuration.Value) self.baseDuration = CustomMando.Value ? CRoundDuration.Value : 0.3f;
                        if (CustomMando.Value) self.force = CRoundForce.Value;
                    }
                    orig(self);
                };
            }
            MandoGaming.MandoLog.LogInfo("Skill changes complete.");
            if (FragCook.Value)
            {
                FragGrenadeCookingImport.CookGrenadeReplace(mandoSL.special.skillFamily, CookGrenade, Overcook);
            }
            if (FragStick.Value)
            {
                SkillFamily skillFamily2 = mandoSL.special.skillFamily;
                Array.Resize<SkillFamily.Variant>(ref skillFamily2.variants, skillFamily2.variants.Length + 1);
                skillFamily2.variants[skillFamily2.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = fragMagnet,
                    unlockableDef = skillFamily2.variants[1].unlockableDef,
                    viewableNode = new ViewablesCatalog.Node(fragMagnet.skillNameToken, false, null)
                };
            }
        }
        public static void GrenadeSetup()
        {
            GameObject grenadeObject = Resources.Load<GameObject>("prefabs/projectiles/commandogrenadeprojectile");
            ProjectileDamage shatterProc = grenadeObject.GetComponent<ProjectileDamage>();
            ProjectileImpactExplosion grenadeImpact = grenadeObject.GetComponent<ProjectileImpactExplosion>();
            if (FragDebuff.Value) shatterProc.damageType = DamageType.BlightOnHit;
            if (FragRadius.Value) grenadeImpact.blastRadius = (CustomMando.Value ? CFragRadius.Value : 14);
            if (FragPhysics.Value)
            {
                PhysicsImpactSpeedModifier fragPhys = grenadeObject.AddComponent<PhysicsImpactSpeedModifier>();
                fragPhys.normalSpeedModifier = 1;
                fragPhys.perpendicularSpeedModifier = 0.3f;
            }
            if (CustomMando.Value)
            {
                grenadeImpact.lifetime = CFragLifetime.Value;
                grenadeImpact.lifetimeAfterImpact = CFragImpactTimer.Value;
                ProjectileSimple grenadePS = grenadeObject.GetComponent<ProjectileSimple>();
                grenadePS.desiredForwardSpeed = CFragSpeed.Value;
                grenadeImpact.blastProcCoefficient = CFragProc.Value;
                grenadeImpact.bonusBlastForce = new Vector3(0, CFragUpwardForce.Value, 0);
            }
            if (FragCook.Value)
            {
                CookGrenade = Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile").InstantiateClone("RiskyRebalanceCommandoNade", true);
                // Taking out most of the added timer-related stuff since cooking has a different effect here
                ProjectileSimple ps = CookGrenade.GetComponent<ProjectileSimple>();
                ps.lifetime = 10f;
                ProjectileImpactExplosion pie = CookGrenade.GetComponent<ProjectileImpactExplosion>();
                ProjectileDamage pd = CookGrenade.GetComponent<ProjectileDamage>();
                // Importing Frag Grenade
                pd.damageType = (ConfigSetup.FragDebuff.Value) ? DamageType.BlightOnHit : DamageType.Generic;
                CookGrenade.AddComponent<States.GrenadeTimer>(); // Using an altered version of GrenadeTimer, see States/GrenadeTimer for more details
                ProjectileAPI.Add(CookGrenade);
                Overcook = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFXCommandoGrenade").InstantiateClone("RiskyRebalanceCommandoNadeOvercookEffect", false);
                EffectComponent ec = Overcook.GetComponent<EffectComponent>();
                ec.soundName = "Play_commando_M2_grenade_explo";
                EffectAPI.AddEffect(new EffectDef(Overcook));
            }
            if (FragStick.Value)
            {
                fragMagnet = ScriptableObject.CreateInstance<SkillDef>();
                fragMagnet.skillNameToken = "Magnet Grenade";
                fragMagnet.skillName = "MGL_STICKY_GRENADE";
                String magnetDesc = "<style=cSub>(beta)</style> ";
                if (StickDebuff.Value) magnetDesc += "<style=cIsDamage>Shattering</style>. ";
                magnetDesc += "Throw a grenade that attaches itself to nearby enemies and explodes for <style=cIsDamage>";
                if (CustomMando.Value && FragDamage.Value) magnetDesc += CFragDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "");
                else if (FragDamage.Value) magnetDesc += "800%";
                else magnetDesc += "700%";
                magnetDesc += " damage</style>. Can hold up to 2.";
                fragMagnet.skillDescriptionToken = magnetDesc;
                fragMagnet.activationStateMachineName = "Weapon";
                fragMagnet.activationState = new SerializableEntityStateType(typeof(ThrowStickyGrenade));
                fragMagnet.baseMaxStock = 2;
                fragMagnet.baseRechargeInterval = 5f;
                fragMagnet.beginSkillCooldownOnSkillEnd = false;
                fragMagnet.canceledFromSprinting = false;
                fragMagnet.fullRestockOnAssign = true;
                fragMagnet.interruptPriority = Resources.Load<GameObject>("prefabs/characterbodies/commandobody").GetComponent<SkillLocator>().special.skillFamily.variants[1].skillDef.interruptPriority;
                fragMagnet.isCombatSkill = true;
                fragMagnet.mustKeyPress = false;
                fragMagnet.rechargeStock = 1;
                fragMagnet.requiredStock = 1;
                fragMagnet.stockToConsume = 1;
                fragMagnet.icon = Resources.Load<GameObject>("prefabs/characterbodies/magebody").GetComponent<SkillLocator>().primary.skillFamily.variants[1].skillDef.icon;
                LoadoutAPI.AddSkillDef(fragMagnet);
                GameObject magnetObject = Resources.Load<GameObject>("prefabs/projectiles/commandostickygrenadeprojectile");
                ProjectileDamage magnetProc = magnetObject.GetComponent<ProjectileDamage>();
                ProjectileImpactExplosion magnetImpact = magnetObject.GetComponent<ProjectileImpactExplosion>();
                if (StickDebuff.Value) magnetProc.damageType = DamageType.BlightOnHit;
                if (StickRadius.Value) magnetImpact.blastRadius = (CustomMando.Value ? CStickRadius.Value : 14);
                magnetImpact.impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/vagranttrackingbombexplosion");
                if (FragPhysics.Value)
                {
                    PhysicsImpactSpeedModifier magnetPhys = magnetObject.AddComponent<PhysicsImpactSpeedModifier>();
                    magnetPhys.normalSpeedModifier = 1;
                    magnetPhys.perpendicularSpeedModifier = 0.3f;
                }
                if (CustomMando.Value)
                {
                    magnetImpact.lifetime = CStickLifetime.Value;
                    magnetImpact.lifetimeAfterImpact = CStickImpactTimer.Value;
                    ProjectileSimple magnetPS = magnetObject.GetComponent<ProjectileSimple>();
                    magnetPS.desiredForwardSpeed = CStickSpeed.Value;
                    magnetImpact.blastProcCoefficient = CStickProc.Value;
                    magnetImpact.bonusBlastForce = new Vector3(0, CStickUpwardForce.Value, 0);
                }
            }
            MandoGaming.MandoLog.LogInfo("Grenade setup complete. Delaying other changes until BodyCatalog.Init...");
        }
        public static void BuffSetup()
        {
            if (FragDebuff.Value)
            {
                shatter = ScriptableObject.CreateInstance<BuffDef>();
                shatter.buffColor = shatterColor;
                shatter.canStack = false;
                shatter.eliteDef = null;
                shatter.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffPulverizeIcon");
                shatter.isDebuff = true;
                shatter.name = "CommandoShatterDebuff";
                BuffAPI.Add(new CustomBuff(shatter));
                On.RoR2.HealthComponent.TakeDamage += new On.RoR2.HealthComponent.hook_TakeDamage(HealthComponent_TakeDamage);
            }
            if (DiveArmor.Value || SlideArmor.Value)
            {
                rollArmor = ScriptableObject.CreateInstance<BuffDef>();
                rollArmor.buffColor = rollColor;
                rollArmor.canStack = false;
                rollArmor.eliteDef = null;
                rollArmor.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield");
                rollArmor.isDebuff = false;
                rollArmor.name = "CommandoArmorBuff";
                BuffAPI.Add(new CustomBuff(rollArmor));
            }
            if (FragDebuff.Value || DiveArmor.Value || SlideArmor.Value) On.RoR2.CharacterBody.RecalculateStats += new On.RoR2.CharacterBody.hook_RecalculateStats(CharacterBody_RecalculateStats);
        }
        public static void Skills(SkillLocator mandoSL)
        {
            if (EnableAltRoll.Value) SkillSetup.AltDive(mandoSL.utility.skillFamily);
            if (EnableAltSlide.Value) SkillSetup.AltSlide(mandoSL.utility.skillFamily);
            MandoGaming.MandoLog.LogInfo("Alt skills done. So far, everything seems stable!");
        }
        private static void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self && self.HasBuff(shatter))
            {
                Reflection.SetPropertyValue(self, "armor", self.armor - (CustomMando.Value ? CShatterDebuff.Value : 20));
            }
            else if (self && self.HasBuff(rollArmor))
            {
                Reflection.SetPropertyValue(self, "armor", self.armor + (CustomMando.Value ? CArmorBuff.Value : 100));
            }
        }
        public static void BoolSetup()
        {
            if (DoubleSpread.Value || CustomMando.Value) DoubleEdit = true;
            if (RoundSize.Value || RoundSpeed.Value || RoundDamage.Value || RoundChain.Value || RoundDuration.Value || CustomMando.Value) RoundEdit = true;
            if (BlastCopium.Value || BlastForce.Value || CustomMando.Value) BlastEdit = true;
            bool DiveCheck = DiveSpeed.Value || DiveArmor.Value || DiveInvincibility.Value || DiveSprint.Value || CustomMando.Value;
            if (DiveCheck && !EnableAltRoll.Value) DiveEdit = true;
            bool SlideCheck = SlideArmor.Value || SlideInvincibility.Value;
            if (SlideCheck && !EnableAltSlide.Value) SlideEdit = true;
            if (SoupDamage.Value || SoupNumber.Value || CustomMando.Value) SoupEdit = true;
            if (FragRadius.Value || FragDebuff.Value || FragPhysics.Value || FragDamage.Value || CustomMando.Value) FragEdit = true;
        }
        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo di)
        {
            bool flag = di.attacker != null;
            if (flag)
            {
                bool flag2 = self != null;
                if (flag2)
                {
                    bool flag3 = self.GetComponent<CharacterBody>() != null;
                    if (flag3)
                    {
                        bool flag4 = di.damageType.HasFlag(DamageType.BlightOnHit);
                        if (flag4)
                        {
                            bool flag5 = di.attacker.GetComponent<CharacterBody>();
                            if (flag5)
                            {
                                bool flag6 = di.attacker.GetComponent<CharacterBody>().baseNameToken == "COMMANDO_BODY_NAME";
                                if (flag6)
                                {
                                    di.damageType = DamageType.Generic;
                                    self.GetComponent<CharacterBody>().AddTimedBuff(shatter, CustomMando.Value ? CShatterTimer.Value : 2.5f);
                                }
                            }
                        }
                    }
                }
            }
            orig(self, di);
        }
        internal static BepInEx.Logging.ManualLogSource MandoLog;
        private static bool DoubleEdit;
        private static bool RoundEdit;
        private static bool BlastEdit;
        private static bool DiveEdit;
        private static bool SlideEdit;
        private static bool SoupEdit;
        private static bool FragEdit;
        public static BuffDef shatter;
        public static BuffDef rollArmor;
        private static readonly Color shatterColor = new Color(0.6f, 0.45f, 0.15f);
        private static readonly Color rollColor = new Color(0.9f, 0.8f, 0.3f);
        public static GameObject mandoPrefab;
        public static GameObject CookGrenade;
        public static GameObject Overcook;
        public static SkillDef fragMagnet;
    }
}
