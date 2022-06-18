using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Projectile : GameObject
    {

        public Data.Skill Data { get; set; }
        public bool IsFinal = false;
        public int shot = 1;
        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

        public override void Update()
        {

        }
    }
}
