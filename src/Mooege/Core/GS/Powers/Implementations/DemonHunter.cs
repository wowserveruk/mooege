/*
 * Copyright (C) 2011 mooege project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Common.Helpers;
using Mooege.Net.GS;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.Attribute;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Net.GS.Message;
using Mooege.Core.GS.Games;
using Mooege.Core.GS.Powers;
using Mooege.Net.GS.Message.Definitions.Text;
using Mooege.Net.GS.Message.Definitions.World;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Net.GS.Message.Definitions.Actor;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Net.GS.Message.Definitions.Player;
using Mooege.Core.GS.Common.Types.Math;

namespace Mooege.Core.GS.Powers.Implementations
{
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.HungeringArrow)]
    public class DemonHunterHungeringArrow : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Spawn Projectile actor
            PowerProjectile powerProjectile = new PowerProjectile(hero.World, 129932, hero.Position, mousePosition, 1f, 3000, 0.5f, 1f, 6f, 3f);

            //Pierce flag, after one pierce you cant pierce again
            float pierced = 0;

            hero.GeneratePrimaryResource(5f);

            //Every 100ms calculate if an impact occure with the projectile and check if the projectile is still alive
            while (powerProjectile.World != null)
            {
                //Check if projectile enter in collision with a monster
                if (powerProjectile.detectCollision())
                {
                    if (powerProjectile.hittedActor.DynamicID != pierced)
                    {
                        powerProjectile.hittedActor.Effect2.addEffect2(162007);
                        powerProjectile.hittedActor.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);

                        //Calculate chance of piecre through
                        if (RandomHelper.Next(0, 100) > 60 || pierced != 0) { powerProjectile.Destroy(); }

                        pierced = powerProjectile.hittedActor.DynamicID;
                    }
                }

                //Each 600 ms seek for monster
                /*if (hero.World.Game.Tick % 36 == 0 && powerProjectile.creationTick < hero.World.Game.Tick - 40 && powerProjectile.World != null)
                {
                    Actor seeked = powerProjectile.World.GetActorsInRange(powerProjectile.getCurrentPosition(), 50f).FirstOrDefault(a => a.ActorType == ActorType.Monster && a.DynamicID != pierced);
                    if (seeked != null)
                    {
                        Vector3D currentPosistion = powerProjectile.getCurrentPosition();
                        PowerProjectile powerProjectile2 = new PowerProjectile(hero.World, 129932, new Vector3D(currentPosistion.X, currentPosistion.Y, seeked.Position.Z), seeked.Position, 1f, 3000, 0.5f, 1f, 6f);
                        powerProjectile.Destroy();
                        powerProjectile = powerProjectile2;
                    }
                }*/

                yield return 100;
            }
        }
    }
}
