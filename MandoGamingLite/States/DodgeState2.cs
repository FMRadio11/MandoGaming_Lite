using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using RoR2;
using EntityStates.Commando;

namespace MandoGamingLite
{
    class DodgeState2 : BaseState
    {
        public override void OnEnter()
        {
            if (ConfigSetup.CustomMando.Value)
            {
                duration = ConfigSetup.CDiveSpeed.Value;
                initialSpeedCoefficient = ConfigSetup.CDiveInitSpeed.Value;
                finalSpeedCoefficient = ConfigSetup.CDiveFnlSpeed.Value;
                stunRadius = ConfigSetup.CDiveStun.Value;
            }
            if (ConfigSetup.DiveSprint.Value) initialSpeedCoefficient = (initialSpeedCoefficient / 1.45f) * base.characterBody.sprintingSpeedMultiplier;
            base.OnEnter();
            Util.PlaySound(DodgeState.dodgeSoundString, base.gameObject);
            this.animator = base.GetModelAnimator();
            ChildLocator component = this.animator.GetComponent<ChildLocator>();
            if (base.isAuthority && base.inputBank && base.characterDirection)
            {
                this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            }
            Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.forwardDirection;
            Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
            float num = Vector3.Dot(this.forwardDirection, rhs);
            float num2 = Vector3.Dot(this.forwardDirection, rhs2);
            this.animator.SetFloat("forwardSpeed", num, 0.1f, Time.fixedDeltaTime);
            this.animator.SetFloat("rightSpeed", num2, 0.1f, Time.fixedDeltaTime);
            if (Mathf.Abs(num) > Mathf.Abs(num2))
            {
                base.PlayAnimation("Body", (num > 0f) ? "DodgeForward" : "DodgeBackward", "Dodge.playbackRate", this.duration + 0.05f);
            }
            else
            {
                base.PlayAnimation("Body", (num2 > 0f) ? "DodgeRight" : "DodgeLeft", "Dodge.playbackRate", this.duration + 0.05f);
            }
            if (DodgeState.jetEffect)
            {
                Transform transform = component.FindChild("LeftJet");
                Transform transform2 = component.FindChild("RightJet");
                if (transform)
                {
                    UnityEngine.Object.Instantiate<GameObject>(DodgeState.jetEffect, transform);
                }
                if (transform2)
                {
                    UnityEngine.Object.Instantiate<GameObject>(DodgeState.jetEffect, transform2);
                }
            }
            if (base.isAuthority)
            {
                BlastAttack rollStun = new BlastAttack
                {
                    baseDamage = 0f,
                    damageType = DamageType.Stun1s,
                    radius = stunRadius,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseForce = 100,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    position = base.characterBody.corePosition,
                    attackerFiltering = AttackerFiltering.Default,
                    procCoefficient = 0f
                };
                rollStun.Fire();
                burst = new List<HurtBox>();
                stunLook = new SphereSearch()
                {
                    mask = LayerIndex.entityPrecise.mask,
                    radius = stunRadius
                };
            }
            this.RecalculateRollSpeed();
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity.y = 0f;
                base.characterMotor.velocity = this.forwardDirection * this.rollSpeed;
            }
            Vector3 b = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
            this.previousPosition = base.transform.position - b;
            if (NetworkServer.active)
            {
                if (ConfigSetup.DiveArmor.Value) base.characterBody.AddBuff(MandoGaming.rollArmor);
                else if (ConfigSetup.DiveInvincibility.Value) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }
        private void RecalculateRollSpeed()
        {
            if (ConfigSetup.DiveSprint.Value) this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(this.initialSpeedCoefficient, this.finalSpeedCoefficient, base.fixedAge / this.duration) * this.outer.commonComponents.characterBody.sprintingSpeedMultiplier;
            else this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(this.initialSpeedCoefficient, this.finalSpeedCoefficient, base.fixedAge / this.duration);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.RecalculateRollSpeed();
            if (burstCheck == true && base.isAuthority)
            {
                burst.Clear();
                stunLook.ClearCandidates();
                stunLook.origin = base.characterBody.corePosition;
                stunLook.RefreshCandidates();
                stunLook.FilterCandidatesByDistinctHurtBoxEntities();
                stunLook.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(base.teamComponent.teamIndex));
                stunLook.GetHurtBoxes(burst);
                bool stunner = false;
                foreach (HurtBox hurtbox in burst)
                {
                    HealthComponent hc = hurtbox.healthComponent;
                    CharacterBody cb = (!hc) ? hc.body : null;
                    if (cb && cb != base.characterBody)
                    {
                        stunner = true;
                    }
                    else
                    {
                        stunner = false;
                    }
                    On.RoR2.SetStateOnHurt.OnTakeDamageServer += (orig, self, dr) =>
                    {
                        orig(self, dr);
                        if (stunner = true && (dr.damageInfo.damageType & DamageType.Stun1s) != DamageType.Generic)
                        {
                            self.SetStun(0.8f);
                        }
                    };
                }
                burstCheck = false;
            }
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = Mathf.Lerp(DodgeState.dodgeFOV, 60f, base.fixedAge / this.duration);
            }
            Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
            if (base.characterMotor && base.characterDirection && normalized != Vector3.zero)
            {
                Vector3 vector = normalized * this.rollSpeed;
                float y = vector.y;
                vector.y = 0f;
                float d = Mathf.Max(Vector3.Dot(vector, this.forwardDirection), 0f);
                vector = this.forwardDirection * d;
                vector.y += Mathf.Max(y, 0f);
                base.characterMotor.velocity = vector;
            }
            this.previousPosition = base.transform.position;
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
        public override void OnExit()
        {
            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.fovOverride = -1f;
            }
            if (NetworkServer.active)
            {
                if (ConfigSetup.DiveArmor.Value) base.characterBody.RemoveBuff(MandoGaming.rollArmor);
                else if (ConfigSetup.DiveInvincibility.Value) base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            burstCheck = true;
            base.OnExit();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public float duration = 0.4f;
        public float initialSpeedCoefficient = 10f;
        public float finalSpeedCoefficient = 2.25f;
        public static float initialHopVelocity = 10f;
        public float stunRadius = 3.5f;
        private float rollSpeed;
        private bool burstCheck = true;
        private SphereSearch stunLook;
        private List<HurtBox> burst;
        private Vector3 forwardDirection;
        private Animator animator;
        private Vector3 previousPosition;
    }
}
