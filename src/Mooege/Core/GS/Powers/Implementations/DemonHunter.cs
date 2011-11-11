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
using Mooege.Core.GS.Map;
using Mooege.Core.GS.Skills;
using Mooege.Core.GS.Ticker.Helpers;

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

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.BolaShot)]
    public class DemonHunterBolaShot : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Show cast effect on player
            hero.Effect2.addEffect2(176854);

            //Regen hatred
            hero.GeneratePrimaryResource(5f);

            //Spawn Projectile actor
            PowerProjectile powerProjectile = new PowerProjectile(hero.World, 77569, hero.Position, mousePosition, 1f, 3000, 0.5f, 1f, 6f, 3f);

            //Every 100ms calculate if an impact occure with the projectile and check if the projectile is still alive
            while (powerProjectile.World != null)
            {
                //Check if projectile enter in collision with a monster
                if (powerProjectile.detectCollision())
                {
                    //Destroy projectile
                    powerProjectile.Destroy();

                    //Play bola constriction effect
                    powerProjectile.hittedActor.Effect2.addEffect2(153874);

                    //Wait 1000 sec b4 explosion
                    yield return 1000;

                    //Play explosion effect 
                    powerProjectile.hittedActor.Effect2.addEffect2(153727);

                    //Deal dmg to main target
                    powerProjectile.hittedActor.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);

                    //Apply aoe effect
                    /*foreach (Actor actor in powerProjectile.hittedActor.World.GetActorsInRange(powerProjectile.hittedActor.Position, 15f))
                    {
                        if (actor.ActorType == ActorType.Monster && actor.DynamicID != powerProjectile.hittedActor.DynamicID)
                            actor.ReceiveDamage(30f, FloatingNumberMessage.FloatType.White);
                    }*/

                }

                yield return 100;
            }
        }
    }

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.EntanglingShot)]
    public class DemonHunterEntanglingShot : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.GeneratePrimaryResource(5f);

            //Spawn Projectile actor
            PowerProjectile powerProjectile = new PowerProjectile(hero.World, 75678, hero.Position, mousePosition, 1f, 3000, 1f, 1f, 6f, 3f);

            //Init list of actor affected by snare
            List<Actor> snaredActor = new List<Actor>();

            //Every 100ms calculate if an impact occure with the projectile and check if the projectile is still alive
            while (powerProjectile.World != null)
            {
                //Check if projectile enter in collision with a monster
                if (powerProjectile.detectCollision())
                {
                    //Destroy projectile
                    powerProjectile.Destroy();

                    //Play hit effect
                    powerProjectile.hittedActor.Effect2.addEffect2(76223);

                    //Receive dmg / Snared
                    powerProjectile.hittedActor.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);
                    powerProjectile.hittedActor.ReceiveDamage(0f, FloatingNumberMessage.FloatType.Snared);

                    //Reduce movement speed of monster to 0
                    //TODO

                    //Check if we can chain effect
                    /*Actor chainActor = powerProjectile.hittedActor.World.GetActorsInRange(powerProjectile.hittedActor.Position, 15f).FirstOrDefault(a => a.ActorType == ActorType.Monster);

                    //We got a chained actor  //76093
                    if (chainActor != null)
                    {
                        chainActor.ReceiveDamage(0f, FloatingNumberMessage.FloatType.Snared);
                        chainActor.Effect2.addEffect2(76227);

                        //Reduce movement speed of monster to 0
                        //TODO
                    }

                    //Add snared target to snared list                    
                    snaredActor.Add(powerProjectile.hittedActor);
                    snaredActor.Add(chainActor);

                }*/

                    yield return 100;
                }

                yield return 2000;

                foreach (Actor actor in snaredActor)
                {
                    //Put back normal movement speed
                    //TODO
                }
            }
        }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.EvasiveFire)]
    public class DemonHunterEvasiveFire : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.GeneratePrimaryResource(10f);

            hero.Effect2.addEffect2(150649);

            if (target != null)
            {
                target.Effect2.addEffect2(150799);
                target.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);

                Actor closeActor = hero.World.GetActorsInRange(hero.Position, 15f).FirstOrDefault(a => a.ActorType == ActorType.Monster);

                //Close actor found, backflip
                if (closeActor != null)
                {
                    //Use discipline
                    hero.UseSecondaryResource(4f);

                    //Get normal inverted vector
                    Vector3D invertedNormVector = PowerUtils.invertVector(PowerUtils.getNormalVector(target.Position, hero.Position));

                    Console.Write(invertedNormVector.X + ":" + invertedNormVector.Y);

                    //Calculate final translate point
                    Vector3D finalPosition = new Vector3D(hero.Position.X + invertedNormVector.X * 15f, hero.Position.Y + invertedNormVector.Y * 15f, hero.Position.Z);

                    //Translate backward
                    hero.Movement2.translateNormal(finalPosition, 69792);
                }
            }

            yield break;
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.Grenades)]
    public class DemonHunterGrenades : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            Vector3D diffVector = new Vector3D(mousePosition.X - hero.Position.X, mousePosition.Y - hero.Position.Y, mousePosition.Z - hero.Position.Z);
            Dictionary<PowerProjectile, Vector3D> grenadeList = new Dictionary<PowerProjectile, Vector3D>();

            //Create 3 grenate projectile
            for (int i = -1; i < 2; i++)
            {
                PowerProjectile projectile = new PowerProjectile(hero.World, 88244, hero.Position, mousePosition, 1f, 3000, 1f, 1f, 6f, 3f, true);

                //Deviate grenade slightly from each other
                Vector3D deviateDiff = diffVector.RotateOnZ(i * RandomHelper.Next(10, 20));

                //FIXME : this prob use translate deth path Sin
                //Calculate final posistion
                Vector3D arcFinalPos = new Vector3D(hero.Position.X + (deviateDiff.X), hero.Position.Y + (deviateDiff.Y), mousePosition.Z);
                grenadeList[projectile] = arcFinalPos;

                //Calculate first leap final posistion
                Vector3D firstArcFinalPos = new Vector3D(hero.Position.X + (deviateDiff.X * 0.8f), hero.Position.Y + (deviateDiff.Y * 0.8f), mousePosition.Z);

                //Translate grenade
                projectile.Movement2.translateArc(firstArcFinalPos, -1, 1.1f);
            }

            //First arc
            yield return 420;

            foreach (KeyValuePair<PowerProjectile, Vector3D> grenade in grenadeList)
            {
                grenade.Key.Movement2.translateArc(grenade.Value, -1, 0.8f);
            }

            yield return 500;

            foreach (KeyValuePair<PowerProjectile, Vector3D> grenade in grenadeList)
            {
                grenade.Key.Effect2.addEffect2(153982);
                foreach (Actor actor in grenade.Key.World.GetActorsInRange(grenade.Key.Position, 5f))
                {
                    if (actor.ActorType == ActorType.Monster)
                    {
                        actor.ReceiveDamage(10f, FloatingNumberMessage.FloatType.White);
                    }
                }
            }

            yield return 200;

            foreach (KeyValuePair<PowerProjectile, Vector3D> grenade in grenadeList)
            {
                grenade.Key.Destroy();
            }

            yield break;
        }
    }*/

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.Impale)]
    public class DemonHunterImpale : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            yield return 100;

            //Regen hatred
            hero.UsePrimaryResource(40f);

            hero.Effect2.addEffect2(150233);

            if (target != null)
            {
                target.Effect2.addEffect2(180667);
                target.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);
            }

            yield break;        
        }
    }

    /*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.FanOfKnives)]
    public class DemonHunterFanOfKinves : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.UsePrimaryResource(20f);

            //Cast effect
            hero.Effect2.addEffect2(77547);

            foreach(Actor actor in hero.World.GetActorsInRange(hero.Position, 10f))
            {
                if (actor.ActorType == ActorType.Monster)
                {
                    actor.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);
                    actor.ReceiveDamage(0f, FloatingNumberMessage.FloatType.AttackSlowed);
                }
            }

            yield break;
        }
    }*/

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.Chakram)]
    public class DemonHunterChakram : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.UsePrimaryResource(20f);

            PowerProjectile projectile = new PowerProjectile(hero.World, 129228, hero.Position, mousePosition, 1f, 3000, 1f, 1f, 0f, 3f, true);
            projectile.Movement2.translateSpiral(mousePosition);
            

            yield break;
        }
    }

    /*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Caltrops)]
    public class DemonHunterCaltrops : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.UseSecondaryResource(8f);

            //Cast effect
            hero.Effect2.addEffect2(131623);

            //Spawn the catual proxy trap
            PowerProxy trap = new PowerProxy(hero.World, 136149, hero.Position);
            trap.Effect2.addEffect2(196032);

            //Calculate trap ttl
            int trapTtl = hero.World.Game.Tick + (60 * 12);

            //List of monster that have already been trapped, monster can only be trapped once by a trap
            List<Actor> trappedTarget = new List<Actor>();

            //Watch for monster proximity for the next 12 sec
            while (hero.World.Game.Tick < trapTtl)
            {
                foreach (Actor actor in trap.World.GetActorsInRange(trap.Position, 10f))
                {
                    if (actor.ActorType == ActorType.Monster && !trappedTarget.Contains(actor))
                    {
                        actor.Effect2.addEffect2(131623);
                        actor.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);
                        actor.ReceiveDamage(0f, FloatingNumberMessage.FloatType.AttackSlowed);
                        trappedTarget.Add(actor);
                    }
                }

                yield return 100;
            }

            //Destroy trap after 12 sec
            trap.Destroy();

            yield break;
        }
    }*/

    /*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Vault)]
    public class DemonHunterVault : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.UseSecondaryResource(8f);

            hero.ActorAttribute.setAttribute(GameAttribute.Power_Buff_0_Visual_Effect_None, new GameAttributeValue(true), Skills.Skills.DemonHunter.Discipline.Vault);

            //Translate backward
            hero.Movement2.translateNormal(mousePosition, 69792, 2f, PowerUtils.getRadian(hero.Position, mousePosition));

            yield break;
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.MarkedForDeath)]
    public class DemonHunterMarkedForDeath : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            if (target == null) { yield break; }

            //Regen hatred
            hero.UseSecondaryResource(8f);

            target.ActorAttribute.setAttribute(GameAttribute.Power_Buff_0_Visual_Effect_None, new GameAttributeValue(true), Skills.Skills.DemonHunter.Discipline.MarkedForDeath);
            //Add dmg buff

            yield return 30000;

            target.ActorAttribute.setAttribute(GameAttribute.Power_Buff_0_Visual_Effect_None, new GameAttributeValue(false), Skills.Skills.DemonHunter.Discipline.MarkedForDeath);
            //Remove dmg buff
        }
    }*/

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.SmokeScreen)]
    public class DemonHunterSmokeScreen : PowerImplementation2
    {
        public override IEnumerable<int> Run(Actor owner, Actor target, Vector3D mousePosition, TargetMessage msg)
        {
            //Cast owner to player
            Mooege.Core.GS.Players.Player hero = owner as Mooege.Core.GS.Players.Player;

            //Regen hatred
            hero.UseSecondaryResource(8f);

            hero.Effect2.addEffect2(131425);
            //Add untargetable attribute

            yield return 2000;

            hero.Effect2.addEffect2(133698);
            //Remove untargetable attribute
        }
    }

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.RapidFire)]
    public class DemonHunterRapidFire : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(60f);

            // HACK: made up demonic meteor spell, not real hydra
            SpawnEffect(185366, TargetPosition);
            yield return WaitSeconds(0.4f);

            IList<Actor> hits = GetTargetsInRange(TargetPosition, 10f);
            WeaponDamage(hits, 10f, DamageType.Fire);
        }
    }

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Vault)]
    public class DemonHunterVault : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(15f);
            //StartCooldown(WaitSeconds(16f));
            SpawnProxy(User.Position).PlayEffectGroup(19352);  // alt cast efg: 170231
            yield return WaitSeconds(0.3f);
            User.Teleport(TargetPosition);
            User.PlayEffectGroup(170232);
        }
    }
}
