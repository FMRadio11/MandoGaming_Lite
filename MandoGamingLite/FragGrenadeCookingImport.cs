using RoR2.Skills;
using EntityStates;
using RoR2.Projectile;
using RoR2;
using R2API;
using System;
using UnityEngine;
using MandoGamingLite.States;

namespace MandoGamingLite
{
    class FragGrenadeCookingImport : ConfigSetup
    {
        // Shoutouts to Moffein for figuring out/making public how this works, it's honestly really cool even if I don't agree with how he balanced it in his mod
        public static void CookGrenadeReplace(SkillFamily SF, GameObject proj, GameObject effect)
        {
            CookGrenade.overcookExplosionEffectPrefab = effect;
            ThrowCookGrenade._projectilePrefab = proj;
            LoadoutAPI.AddSkill(typeof(CookGrenade));
            LoadoutAPI.AddSkill(typeof(ThrowCookGrenade));
            SkillDef grenadeDef = SkillDef.CreateInstance<SkillDef>();
            String fragDesc = "";
            if (FragDebuff.Value)
            {
                LanguageAPI.Add("KEYWORD_SHATTERING", "<style=cKeywordName>Shattering</style><style=cSub><style=cIsDamage>Reduces armor by -25</style> for 2.5 seconds.</style>");
                fragDesc = fragDesc + "<style=cIsDamage>Shattering</style>. ";
                grenadeDef.keywordTokens = new string[] { "KEYWORD_SHATTERING" };
            }
            fragDesc += "Throw a grenade that explodes for <style=cIsDamage>";
            if (CustomMando.Value && FragDamage.Value) fragDesc += CFragDamage.Value.ToString("P0").Replace(" ", "").Replace(",", "");
            else if (FragDamage.Value) fragDesc += "800%";
            else fragDesc += "700%";
            fragDesc += " damage</style>; can be held to reduce delay before exploding. Hold up to 2.";
            LanguageAPI.Add("COOK_COMMANDO_SPECIAL_ALT1_DESC", fragDesc);
            grenadeDef.activationState = new SerializableEntityStateType(typeof(CookGrenade));
            grenadeDef.activationStateMachineName = "Weapon";
            grenadeDef.baseMaxStock = 2;
            grenadeDef.baseRechargeInterval = 5f;
            grenadeDef.beginSkillCooldownOnSkillEnd = true;
            grenadeDef.canceledFromSprinting = false;
            grenadeDef.dontAllowPastMaxStocks = true;
            grenadeDef.forceSprintDuringState = false;
            grenadeDef.fullRestockOnAssign = true;
            grenadeDef.icon = SF.variants[1].skillDef.icon;
            grenadeDef.interruptPriority = InterruptPriority.PrioritySkill;
            grenadeDef.isCombatSkill = true;
            grenadeDef.keywordTokens = new string[] { };
            grenadeDef.mustKeyPress = false;
            grenadeDef.cancelSprintingOnActivation = true;
            grenadeDef.rechargeStock = 1;
            grenadeDef.requiredStock = 1;
            grenadeDef.skillName = "Grenade";
            grenadeDef.skillNameToken = "COMMANDO_SPECIAL_ALT1_NAME";
            grenadeDef.skillDescriptionToken = "COOK_COMMANDO_SPECIAL_ALT1_DESC";
            grenadeDef.stockToConsume = 1;
            LoadoutAPI.AddSkillDef(grenadeDef);
            SF.variants[1].skillDef = grenadeDef;
        }
    }
}
