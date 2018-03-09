using UnityEngine;

namespace Bomberman
{
    public abstract class Powerup : MonoBehaviour
    {
        /// <summary>
        /// duration the bomb is active for
        /// </summary>
        public float lifetime = 5f;

        /// <summary>
        /// 
        /// </summary>
        public float multiplier = 1.5f;

        /// <summary>
        /// copy values from powerup go
        /// </summary>
        public virtual void Setup( float multiplier, float lifetime )
        {
            this.multiplier = multiplier;
            this.lifetime = lifetime;
        }

        /// <summary>
        /// initialize and active powerup
        /// </summary>
        public abstract void OnPickup();

        /// <summary>
        /// implements powerup effect
        /// </summary>
        protected abstract System.Collections.IEnumerator Activate();
    }
}