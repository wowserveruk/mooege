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
using Mooege.Core.GS.Actors;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Core.GS.Ticker.Helpers;
using Mooege.Net.GS;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.Attribute;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Net.GS.Message;
using Mooege.Core.GS.Games;
using Mooege.Core.GS.Powers;
using Mooege.Net.GS.Message.Definitions.World;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Net.GS.Message.Definitions.Actor;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Net.GS.Message.Definitions.Player;

namespace Mooege.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.Bash)]
    public class BarbarianBash : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(0.25f); // wait for swing animation

            User.PlayEffectGroup(18662);

            if (CanHitMeleeTarget(Target))
            {
                GeneratePrimaryResource(6f);
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 1.45f, DamageType.Physical);
            }

            yield break;
        }
    }

    [ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.Cleave)]
    public class BarbarianCleave : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(0.25f); // wait for swing animation

            User.PlayEffectGroup(18671);
			
			if (CanHitMeleeTarget(Target))
            {
                GeneratePrimaryResource(4f);
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 1.45f, DamageType.Physical);
            }

            yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.GroundStomp)]
    public class BarbarianGroundStomp : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(0.25f); // wait for swing animation
			
			StartCooldown(WaitSeconds(12f));

            User.PlayEffectGroup(18685);

            if (CanHitMeleeTarget(Target))
            {
                GeneratePrimaryResource(20f);
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 1.45f, DamageType.Physical);
            }

            yield break;
        }
    }

    [ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.LeapAttack)]
    public class BarbarianLeap : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            //StartCooldown(WaitSeconds(10f));

            Vector3D delta = new Vector3D(TargetPosition.X - User.Position.X, TargetPosition.Y - User.Position.Y,
                                          TargetPosition.Z - User.Position.Z);
            float delta_length = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            Vector3D delta_normal = new Vector3D(delta.X / delta_length, delta.Y / delta_length, delta.Z / delta_length);
            float unitsMovedPerTick = 30f;
            Vector3D ramp = new Vector3D(delta_normal.X * (delta_length / unitsMovedPerTick),
                                         delta_normal.Y * (delta_length / unitsMovedPerTick),
                                         1.48324f); // usual leap height, possibly different when jumping up/down?

            // TODO: Generalize this and put it in Actor
            User.World.BroadcastIfRevealed(new ACDTranslateArcMessage()
            {
                ActorId = (int)User.DynamicID,
                Start = User.Position,
                Velocity = ramp,
                Field3 = 303110, // used for male barb leap
                FlyingAnimationTagID = 69792, // used for male barb leap
                LandingAnimationTagID = -1,
                Field6 = -0.1f, // leap falloff
                Field7 = Skills.Skills.Barbarian.FuryGenerators.LeapAttack,
                Field8 = 0
            }, User);
            User.Position = TargetPosition;

            // wait for leap to hit
            yield return WaitSeconds(0.65f);

            // ground smash effect
            User.PlayEffectGroup(18688);

            bool hitAnything = false;
            foreach (Actor actor in GetTargetsInRange(TargetPosition, 8f))
            {
                hitAnything = true;
                WeaponDamage(actor, 0.70f, DamageType.Physical);
            }

            if (hitAnything)
                GeneratePrimaryResource(20f);

            yield break;
        }
    }

    [ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.Whirlwind)]
    public class BarbarianWhirlwind : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(14f);

            //User.AddBuff(new WhirlWindEffectBuff(WaitSeconds(0.250f)));

            foreach (Actor target in GetTargetsInRange(User.Position, 9f))
            {
                WeaponDamage(target, 0.44f, DamageType.Physical);
            }

            yield break;
        }
    }

    [ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.AncientSpear)]
    public class BarbarianAncientSpear : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            StartCooldown(WaitSeconds(10f));
            
            var projectile = new PowerProjectile(User.World, 74636, User.Position, TargetPosition, 2f, 500f, 1f, 3f, 5f, 0f);

            User.AddRopeEffect(79402, projectile);

            projectile.OnHit = () =>
            {
                GeneratePrimaryResource(15f);

                var inFrontOfUser = PowerMath.ProjectAndTranslate2D(User.Position, projectile.hittedActor.Position,
                    User.Position, 5f);

                _setupReturnProjectile(projectile.hittedActor.Position, 5f);

                // GET OVER HERE
                projectile.hittedActor.TranslateNormal(inFrontOfUser, 2f);
                WeaponDamage(projectile.hittedActor, 1.00f, DamageType.Physical);

                projectile.Destroy();
            };

            projectile.OnTimeout = () =>
            {
                _setupReturnProjectile(projectile.getCurrentPosition(), 0f);
            };

            yield break;
        }

        private void _setupReturnProjectile(Vector3D spawnPosition, float heightOffset)
        {
            var return_proj = new PowerProjectile(User.World, 79400, spawnPosition,
                User.Position, 2f, 500f, 1f, 3f, heightOffset, 0f);

            User.AddRopeEffect(79402, return_proj);

            return_proj.OnUpdate = () =>
            {
                if (PowerMath.Distance(return_proj.getCurrentPosition(), User.Position) < 15f) // TODO: make this tick based distance?
                {
                    return_proj.Destroy();
                    return false;
                }
                return true;
            };
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.Frenzy)]
    public class BarbarianFrenzy : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {            
            //increaseAttackSeep(0.15f);

            if (CanHitMeleeTarget(Target))
            {
                GeneratePrimaryResource(3f);
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 1.45f, DamageType.Physical);
            }

            
            //decreaseAttackSeep(0.15f);
            
			yield break;
        }           
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.HammerOfTheAncients)]
    public class BarbarianHammerOfTheAncients : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            User.PlayEffectGroup(18705);

            if (CanHitMeleeTarget(Target))
            {
                UsePrimaryResource(20f);
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 1.45f, DamageType.Physical);
            }

            yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.ThreateningShout)]
    public class BarbarianThreateningShout : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {           
            UsePrimaryResource(20f);
			
			User.PlayEffectGroup(158731);

            yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.BattleRage)]
    public class BarbarianBattleRage : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
			User.PlayEffectGroup(18666);
			
			UsePrimaryResource(20f);

            yield break;
        }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.WeaponThrow)]
    public class BarbarianWeaponThrow : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
			UseResources(20f);

            //Synchronize with animation
            yield return 100;
            
            //Spawn Projectile actor
            PowerProjectile powerProjectile = new PowerProjectile(hero.World, 163462, hero.Position, mousePosition, 1f, 3000, 1f, 3f, 0f, 3f);
            
            //Every 100ms calculate if an impact occure with the projectile and check if the projectile is still alive
            while (powerProjectile.World != null)
            {
                //Check if projectile enter in collision with a monster
                if (powerProjectile.detectCollision())
                {   
                    powerProjectile.hittedActor.Effect.addEffect(18707);
                    powerProjectile.hittedActor.ReceiveDamage(50f, FloatingNumberMessage.FloatType.White);
                    powerProjectile.Destroy();
                }

                yield return 100;
            }
        }
    }*/
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.Rend)]
    public class BarbarianRend : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            User.PlayEffectGroup(70614); 

			if (CanHitMeleeTarget(Target))
            {
                UsePrimaryResource(20f);
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 1.45f, DamageType.Physical);
            }

            yield break;
        }
    }   
}
