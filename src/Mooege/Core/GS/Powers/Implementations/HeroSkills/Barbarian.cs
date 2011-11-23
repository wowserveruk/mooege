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
using Mooege.Core.GS.Common.Types.Misc;
using Mooege.Core.GS.Ticker;
using Mooege.Net.GS.Message;
using Mooege.Core.GS.Common.Types.TagMap;

namespace Mooege.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.Bash)]		//Level 1
    public class BarbarianBash : PowerScript
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
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.HammerOfTheAncients)]		//Level 2
    public class BarbarianHammerOfTheAncients : PowerScript
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
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.ThreateningShout)]		//Level 2
    public class BarbarianThreateningShout : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {           
            UsePrimaryResource(20f);
			
			User.PlayEffectGroup(158731);

            yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.BattleRage)]		//Level 3
    public class BarbarianBattleRage : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
			User.PlayEffectGroup(18666);
			
			UsePrimaryResource(20f);

            yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.Cleave)]		//Level 4
    public class BarbarianCleave : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(0.25f); // wait for swing animation

            //Cleave have a different animation based on the swing side wich is cointain in the Animpreplay data of the target message
            //if (this.Message.Field6.Field0 == 1)            
                User.PlayEffectGroup(18671);            
            //else            
                User.PlayEffectGroup(18672);            

            GeneratePrimaryResource(5f);

            foreach (Actor actor in GetTargetsInRange(User.Position, 12f))
            {
                if (PowerMath.PointInBeam(actor.Position, User.Position, PowerMath.ProjectAndTranslate2D(User.Position, TargetPosition, User.Position, 15f), 15f))
                {
                    WeaponDamage(actor, 1.20f, DamageType.Physical, true);
                }
            }

            yield break;
        }

        private IEnumerable<Actor> GetTargetsInRange(Vector3D vector3D, float p)
        {
            throw new NotImplementedException();
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.LeapAttack)]		//Level 6
    public class BarbarianLeap : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            //StartCooldown(WaitSeconds(10f));

            Vector3D delta = new Vector3D(TargetPosition - User.Position);
            float delta_length = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            Vector3D delta_normal = new Vector3D(delta.X / delta_length, delta.Y / delta_length, delta.Z / delta_length);
            float unitsMovedPerTick = 30f;
            Vector3D ramp = new Vector3D(delta_normal.X * (delta_length / unitsMovedPerTick),
                                         delta_normal.Y * (delta_length / unitsMovedPerTick),
                                         1.483239f); // usual leap height, possibly different when jumping up/down?

            // TODO: Generalize this and put it in Actor
            User.World.BroadcastIfRevealed(new ACDTranslateArcMessage()
            {
                ActorId = (int)User.DynamicID,
                Start = User.Position,
                Velocity = ramp,
                //Field3 = 303110, // used for male barb leap
                FlyingAnimationTagID = 69792, // used for male barb leap
                LandingAnimationTagID = -1,
                Field6 = -0.1f, // gravity
                Field7 = Skills.Skills.Barbarian.FuryGenerators.LeapAttack,
                Field8 = 0,
                Field9 = TargetPosition.Z,
            }, User);
            User.Position = TargetPosition;

            // wait for leap to hit
            yield return WaitSeconds(0.6f);

            // ground smash effect
            User.PlayEffectGroup(18688);

            bool hitAnything = false;
            foreach (Actor actor in GetEnemiesInRange(TargetPosition, 8f))
            {
                hitAnything = true;
                WeaponDamage(actor, 0.70f, DamageType.Physical);
            }

            if (hitAnything)
                GeneratePrimaryResource(15f);

            yield break;
        }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.WeaponThrow)]		//Level 7
    public class BarbarianWeaponThrow : PowerScript
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
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.GroundStomp)]		//Level 8
    public class BarbarianGroundStomp : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            yield return WaitSeconds(0.200f); // wait for swing animation

            User.PlayEffectGroup(18685);

            bool hitAnything = false;

            foreach (Actor actor in GetTargetsInRange(User.Position, 17f))
            {
                WeaponDamage(actor, 0.7f, DamageType.Physical, true);
                Stunt(actor);
                hitAnything = true;
            }

            if(hitAnything)
                GeneratePrimaryResource(15f);

            //FIXME
            //Add power cooldown

            yield break;
        }

        private IEnumerable<Actor> GetTargetsInRange(Vector3D vector3D, float p)
        {
            throw new NotImplementedException();
        }

        private void Stunt(Actor actor)
        {
            throw new NotImplementedException();
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.Rend)]		//Level 9
    public class BarbarianRend : PowerScript
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
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.Frenzy)]		//Level 11
    public class BarbarianFrenzy : PowerScript
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

    [ImplementsPowerSNO(Skills.Skills.Barbarian.FuryGenerators.AncientSpear)]		//Level 22
    public class BarbarianAncientSpear : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            //StartCooldown(WaitSeconds(10f));

            var projectile = new Projectile(this, 74636, User.Position);
            projectile.Timeout = WaitSeconds(0.5f);
            projectile.OnHit = (hit) =>
            {
                GeneratePrimaryResource(15f);

                var inFrontOfUser = PowerMath.ProjectAndTranslate2D(User.Position, hit.Position,
                    User.Position, 5f);

                _setupReturnProjectile(hit.Position);

                // GET OVER HERE
                hit.TranslateNormal(inFrontOfUser, 2f);
                WeaponDamage(hit, 1.00f, DamageType.Physical);

                projectile.Destroy();
            };
            projectile.OnTimeout = () =>
            {
                _setupReturnProjectile(projectile.Position);
            };

            projectile.Launch(TargetPosition, 2f);
            User.AddRopeEffect(79402, projectile);

            yield break;
        }

        private void _setupReturnProjectile(Vector3D spawnPosition)
        {
            var return_proj = new Projectile(this, 79400, new Vector3D(spawnPosition.X, spawnPosition.Y, User.Position.Z));
            Vector3D prevPosition = return_proj.Position;
            return_proj.OnUpdate = () =>
            {
                if (PowerMath.Distance2D(return_proj.Position, User.Position) < 15f)
                    return_proj.Destroy();
            };

            return_proj.Launch(User.Position, 2f);
            User.AddRopeEffect(79402, return_proj);
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.Barbarian.FurySpenders.Whirlwind)]		//Level 27
    public class BarbarianWhirlwind : PowerScript
    {
        public override IEnumerable<TickTimer> Run()
        {
            AddBuff(User, new WhirlwindEffect());
            yield break;
        }

        [ImplementsBuffSlot(0)]
        public class WhirlwindEffect : PowerBuff
        {
            private TickTimer _damageTimer;
            private TickTimer _tornadoSpawnTimer;

            public override void Init()
            {
                Timeout = WaitSeconds(0.20f);
            }

            public override bool Update()
            {
                if (base.Update())
                    return true;

                if (_damageTimer == null || _damageTimer.TimedOut)
                {
                    _damageTimer = WaitSeconds(ScriptFormula(0));
                    //UsePrimaryResource(EvalTag(PowerKeys.ResourceCost));

                    foreach (Actor target in GetEnemiesInRange(User.Position, ScriptFormula(2)))
                    {
                        WeaponDamage(target, ScriptFormula(1), Rune_A > 0 ? DamageType.Fire : DamageType.Physical);
                    }
                }

                if (Rune_B > 0)
                {
                    // spawn tornado projectiles in random directions every timed period
                    if (_tornadoSpawnTimer == null)
                        _tornadoSpawnTimer = WaitSeconds(ScriptFormula(5));

                    if (_tornadoSpawnTimer.TimedOut)
                    {
                        _tornadoSpawnTimer = WaitSeconds(ScriptFormula(5));

                        var tornado = new Projectile(this, 162386, User.Position);
                        tornado.Timeout = WaitSeconds(3f);
                        tornado.OnHit = (hit) =>
                        {
                            WeaponDamage(hit, ScriptFormula(6), DamageType.Physical);
                        };
                        tornado.Launch(new Vector3D(User.Position.X + (float)Rand.NextDouble() - 0.5f,
                                                    User.Position.Y + (float)Rand.NextDouble() - 0.5f,
                                                    User.Position.Z), 0.25f);
                    }
                }

                return false;
            }
        }
    }
}
