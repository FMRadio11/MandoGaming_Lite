using R2API.Networking;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using System;

namespace MandoGamingLite.States
{
    public class CookGrenade : BaseState
    {
        public static float totalFuseTime = 1.25f;
        public static string beepSoundString = "Play_commando_M2_grenade_beep";

        //Things that happen when you overcook
        public static float selfHPDamagePercent = 0.2f; //Remove this fraction of combined HP
        public static float selfForce = 4500f;
        public static GameObject overcookExplosionEffectPrefab;
        private bool flashPlayed = true;
        public static GameObject grenadeIndicatorPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashFMJ");

        private float stopwatch;
        private bool grenadeThrown = false;
        private Animator animator;
        private Transform rightHand;
        private bool overcookSet = false;
        private float overcookTimer;
        private float setTimer = 0.75f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (ConfigSetup.CustomMando.Value)
            {
                setTimer = ConfigSetup.CCookChargeTimer.Value;
                totalFuseTime = ConfigSetup.CCookOverTimer.Value;
                selfForce = ConfigSetup.CCookSelfForce.Value;
                selfHPDamagePercent = ConfigSetup.CCookSelfDamage.Value;
            }
            setTimer /= this.attackSpeedStat;
            animator = base.GetModelAnimator();
            if (animator)
            {
                base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", totalFuseTime * 4.5f);
                base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", totalFuseTime * 4.5f);
            }

            ChildLocator childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                rightHand = childLocator.FindChild("HandR");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (overcookSet) overcookTimer += Time.fixedDeltaTime;
            if (overcookTimer >= 0.75f)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            if (stopwatch >= setTimer && base.fixedAge < totalFuseTime && flashPlayed)
            {
                Util.PlaySound(beepSoundString, base.gameObject);
                if (rightHand)
                {
                    EffectManager.SpawnEffect(grenadeIndicatorPrefab, new EffectData { origin = rightHand.position }, false);
                }
                flashPlayed = false;
            }

            if (base.isAuthority && !overcookSet)
            {
                base.StartAimMode(2f);
                if (base.inputBank && base.inputBank.skill4.down)
                {
                    if (base.fixedAge > totalFuseTime)
                    {
                        if (base.characterBody && base.healthComponent)
                        {
                            OvercookExplosion();
                        }
                        overcookSet = true;
                        // this.outer.SetNextStateToMain();
                        return;
                    }
                }
                else
                {
                    SwapToThrowGrenade();
                    return;
                }
            }
        }

        public virtual void SwapToThrowGrenade()
        {
            grenadeThrown = true;
            this.outer.SetNextState(new ThrowCookGrenade { fuseTime = Math.Max(0.75f - base.fixedAge, 0) });
        }

        public override void OnExit()
        {
            if (!grenadeThrown && animator)
            {
                base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", 0.5f);
                base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", 0.5f);
            }
            base.OnExit();
        }

        private void OvercookExplosion()
        {
            EffectManager.SpawnEffect(overcookExplosionEffectPrefab, new EffectData { origin = base.transform.position, scale = 11f }, true);
            new BlastAttack
            {
                radius = ConfigSetup.CustomMando.Value ? ConfigSetup.CCookOopsRadius.Value : 11f,
                attackerFiltering = AttackerFiltering.NeverHit,
                baseDamage = this.damageStat * ((ConfigSetup.CustomMando.Value && ConfigSetup.CCookOopsDamage.Value != ConfigSetup.CFragDamage.Value) ? ConfigSetup.CCookOopsDamage.Value : ThrowCookGrenade._damageCoefficient),
                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                procCoefficient = (ConfigSetup.CustomMando.Value) ? ConfigSetup.CCookOopsProc.Value : 1f,
                baseForce = ((ConfigSetup.CustomMando.Value && ConfigSetup.CCookOopsForce.Value != ConfigSetup.CFragForce.Value) ? ConfigSetup.CCookOopsForce.Value : ThrowCookGrenade._force),
                crit = base.RollCrit(),
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                position = base.transform.position,
                teamIndex = base.teamComponent.teamIndex,
                procChainMask = default(ProcChainMask)
            }.Fire();
            DamageInfo selfDamage = new DamageInfo
            {
                attacker = null,
                crit = false,
                damage = base.healthComponent.fullCombinedHealth * CookGrenade.selfHPDamagePercent,
                force = base.GetAimRay().direction * CookGrenade.selfForce,
                damageType = DamageType.AOE,
                inflictor = null,
                procCoefficient = 0f,
                procChainMask = default(ProcChainMask),
                position = base.transform.position
            };
            NetworkingHelpers.DealDamage(selfDamage, base.characterBody.mainHurtBox, true, false, false);

            base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", 0.25f);
            base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", 0.25f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
