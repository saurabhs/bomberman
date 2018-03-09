using System.Collections;
using UnityEngine;

namespace Bomberman
{
    public class ExtraBomb : Powerup
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
        protected override IEnumerator Activate()
        {
            var playerController = GetComponentInParent<PlayerController>();
            playerController.bombsInHand++;

            yield return new WaitForSeconds( lifetime );

            playerController.bombsInHand = 1;
            playerController.powerupName = string.Empty;
        }
    }
}