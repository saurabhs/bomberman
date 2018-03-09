using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bomberman
{
    public class LongerBomb : Powerup
    {
        /// <summary>
        /// initailize the playerController 
        /// ref and  starts activation
        /// </summary>
        public override void OnPickup()
        {
            StartCoroutine( Activate() );
        }

        /// <summary>
        /// activate powerup effects
        /// </summary>
        protected override IEnumerator Activate()
        {
            var playerController = GetComponentInParent<PlayerController>();
            playerController.rangeMultiplier = multiplier;

            yield return new WaitForSeconds( lifetime );

            playerController.rangeMultiplier = 1f;
            playerController.powerupName = string.Empty;
        }
    }
}