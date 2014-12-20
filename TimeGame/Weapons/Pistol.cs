using System;

namespace TimeGame
{
    public class Pistol : Weapon
    {
        public Pistol() : base()
        {
            fireType = FireType.Semi;
            shotSpeed = 100;
            bulletsInShot = 1;
            bulletsInClip = 12;
            shotSpread = 0;
            damageInBullet = 4;
            rateOfFire = new Tuple<int, int>(1100, 1200);
            reloadTime = 1000;
        }
    }
}
