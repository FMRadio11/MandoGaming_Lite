using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using EntityStates.Commando;

namespace MandoGamingLite
{
    class SlideState2 : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(SlideState.soundString, base.gameObject);
            if (base.isAuthority && base.inputBank && base.characterDirection)
            {
                base.characterDirection.forward = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            }
            if (base.characterMotor)
            {
                startedStateGrounded = base.characterMotor.isGrounded;
            }
            if (SlideState.jetEffectPrefab)
            {
                Transform transform = base.FindModelChild("LeftJet");
                Transform transform2 = base.FindModelChild("RightJet");
                if (transform)
                {
                    UnityEngine.Object.Instantiate<GameObject>(SlideState.jetEffectPrefab, transform);
                }
                if (transform2)
                {
                    UnityEngine.Object.Instantiate<GameObject>(SlideState.jetEffectPrefab, transform2);
                }
            }
            base.characterBody.SetSpreadBloom(0f, false);
            if (!startedStateGrounded)
            {
                base.PlayAnimation("Body", "Jump");
                Vector3 velocity = base.characterMotor.velocity;
                velocity.y = base.characterBody.jumpPower * (ConfigSetup.CustomMando.Value ? ConfigSetup.CSlideJump.Value : 1.2f);
                base.characterMotor.velocity = velocity;
                return;
            }
            base.PlayAnimation("Body", "SlideForward", "SlideForward.playbackRate", SlideState.slideDuration);
            if (SlideState.slideEffectPrefab)
            {
                Transform parent = base.FindModelChild("Base");
                this.slideEffectInstance = UnityEngine.Object.Instantiate<GameObject>(SlideState.slideEffectPrefab, parent);
            }
            if (NetworkServer.active && startedStateGrounded)
            {
                if (ConfigSetup.SlideArmor.Value) base.characterBody.AddBuff(MandoGaming.rollArmor);
                else if (ConfigSetup.SlideInvincibility.Value) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                float num = this.startedStateGrounded ? SlideState.slideDuration : SlideState.jumpDuration;
                if (base.inputBank && base.characterDirection)
                {
                    base.characterDirection.moveVector = base.inputBank.moveVector;
                    this.forwardDirection = base.characterDirection.forward;
                }
                if (base.characterMotor)
                {
                    float num2;
                    if (this.startedStateGrounded)
                    {
                        num2 = SlideState.forwardSpeedCoefficientCurve.Evaluate(base.fixedAge / num);
                    }
                    else
                    {
                        num2 = SlideState.jumpforwardSpeedCoefficientCurve.Evaluate(base.fixedAge / num);
                    }
                    base.characterMotor.rootMotion += num2 * this.moveSpeedStat * this.forwardDirection * Time.fixedDeltaTime;
                }
                if (base.fixedAge >= num)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }
        public override void OnExit()
        {
            this.PlayImpactAnimation();
            if (this.slideEffectInstance)
            {
                EntityState.Destroy(this.slideEffectInstance);
            }
            if (base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            if (base.characterBody.HasBuff(MandoGaming.rollArmor))
            {
                base.characterBody.RemoveBuff(MandoGaming.rollArmor);
            }
            base.OnExit();
        }
        private void PlayImpactAnimation()
        {
            Animator modelAnimator = base.GetModelAnimator();
            int layerIndex = modelAnimator.GetLayerIndex("Impact");
            if (layerIndex >= 0)
            {
                modelAnimator.SetLayerWeight(layerIndex, 1f);
            }
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        public static float slideDuration = SlideState.slideDuration;
        public static float jumpDuration = SlideState.jumpDuration;
        private Vector3 forwardDirection;
        private GameObject slideEffectInstance;
        private bool startedStateGrounded;
    }
}
