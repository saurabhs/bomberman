using System;
using System.Collections;
using UnityEngine;

namespace Bomberman
{
    public class FastMove : Powerup
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
            var lastSpeed = playerController.speed;
            playerController.speed = lastSpeed * multiplier;

            yield return new WaitForSeconds( lifetime );

            playerController.speed = lastSpeed;
            playerController.powerupName = string.Empty;
        }
    }
}