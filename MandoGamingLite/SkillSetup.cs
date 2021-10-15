using System;
using RoR2;
using EntityStates;
using UnityEngine;
using RoR2.Projectile;
using RoR2.Skills;
using R2API;

namespace MandoGamingLite
{
    class SkillSetup : ConfigSetup
    {
        public static void AltDive(SkillFamily SF)
        {
            SkillDef skillDef = SF.variants[0].skillDef;
            skillDef.activationState = new SerializableEntityStateType(typeof(DodgeState2));
            LanguageAPI.Add("KEYWORD_ROLL", "<style=cKeywordName>Stunning</style><style=cSub>This skill deals no damage and cannot proc items, but will briefly stun enemies adjacent to Commando at the beginning of the roll.</style>");
            skillDef.keywordTokens = new string[] { "KEYWORD_ROLL" };
        }
        public static void AltSlide(SkillFamily SF)
        {
            SkillDef skillDef = SF.variants[1].skillDef;
            skillDef.activationState = new SerializableEntityStateType(typeof(SlideState2));
        }
    }
}
