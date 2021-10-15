using RoR2.Projectile;
using UnityEngine;

namespace MandoGamingLite.States
{
    public class GrenadeTimer : MonoBehaviour
    {
        public void Start()
        {
            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            ProjectileImpactExplosion pie = base.GetComponent<ProjectileImpactExplosion>();
            if (pd && pie)
            {
                // Instead of the original effect of setting the internal stopwatch to how long it was cooked, this instead reduces the impact lifetime to anywhere between the vanilla 1s to 0.25s at full charge
                pie.lifetimeAfterImpact = pd.force + 0.25f; 
                pd.force = ThrowCookGrenade._force;
            }
            Destroy(this);
        }
    }
}
