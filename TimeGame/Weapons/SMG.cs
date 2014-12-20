using System;

namespace TimeGame
{
    public class SMG : Weapon
    {
        public SMG() : base()
        {
            fireType = FireType.Auto;
            shotSpeed = 100;
            bulletsInShot = 1;
            bulletsInClip = 30;
            shotSpread = 10;
            damageInBullet = 4;
            rateOfFire = new Tuple<int, int>(700, 900);
            reloadTime = 2000;
        }
    }
}
