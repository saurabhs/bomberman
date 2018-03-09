using UnityEngine;

namespace Bomberman
{
    public class RemoteBomb : Powerup
    {
        /// <summary>
        /// 
        /// </summary>
        public override void OnPickup()
        {
            StartCoroutine( Activate() );
        }

        /// <summary>
        /// 
        /// </summary>
        protected override System.Collections.IEnumerator Activate()
        {
            var playerController = GetComponentInParent<PlayerController>();
            playerController.hasRemoteBomb = true;

            yield return new WaitForSeconds( lifetime );

            playerController.hasRemoteBomb = false;
            playerController.powerupName = string.Empty;
        }
    }
}