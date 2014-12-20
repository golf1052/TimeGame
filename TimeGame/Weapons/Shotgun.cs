using System;

namespace TimeGame
{
    public class Shotgun : Weapon
    {
        public Shotgun() : base()
        {
            fireType = FireType.Semi;
            shotSpeed = 100;
            bulletsInShot = 10;
            bulletsInClip = 10;
            shotSpread = 10;
            damageInBullet = 4;
            rateOfFire = new Tuple<int, int>(300, 500);
            reloadTime = 5000;
        }
    }
}
