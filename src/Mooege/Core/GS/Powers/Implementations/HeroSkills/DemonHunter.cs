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
using Mooege.Core.GS.Ticker;

namespace Mooege.Core.GS.Powers.Implementations
{
    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.HungeringArrow)]		// Level 1
    public class DemonHunterHungeringArrow : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(10f);
            
            var projectile = new PowerProjectile(User.World, 129932, User.Position, TargetPosition, 1f, 3000, 0.5f, 1f, 6f, 3f);
            projectile.OnHit = () =>
            {
                SpawnEffect(162007, projectile.getCurrentPosition()); // impact effect
                projectile.Destroy();
                WeaponDamage(projectile.hittedActor, 50f, DamageType.Arcane);
            };

            yield break;
        }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.EvasiveFire)]		// Level 2
    public class DemonHunterEvasiveFire : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            GeneratePrimaryResource(4f);
			
			var projectile = new PowerProjectile(User.World, 150649, User.Position, TargetPosition, 1f, 3000, 1f, 1f, 6f, 3f);
            projectile.OnHit = () =>
            {
                SpawnEffect(150799, projectile.getCurrentPosition()); // impact effect
                projectile.Destroy();
                WeaponDamage(projectile.hittedActor, 50f, DamageType.Arcane);
            };
			
			yield break;
        }
    }*/
	
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.Impale)]		// Level 2
    public class DemonHunterImpale : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(6f);
			
			var projectile = new PowerProjectile(User.World, 150233, User.Position, TargetPosition, 1f, 3000, 1f, 1f, 6f, 3f);
            projectile.OnHit = () =>
            {
                SpawnEffect(180667, projectile.getCurrentPosition()); // impact effect
                projectile.Destroy();
                WeaponDamage(projectile.hittedActor, 50f, DamageType.Arcane);
            };

            yield break;        
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Caltrops)]		// Level 3
    public class DemonHunterCaltrops : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
			UseSecondaryResource(8f);

            User.PlayEffectGroup(131623);
			
			var trap = new PowerProjectile(User.World, 136149, User.Position, TargetPosition, 1f, 3000, 1f, 1f, 0f, 3f);
            trap.OnHit = () =>
            {
                SpawnEffect(196032, trap.getCurrentPosition()); // impact effect
                trap.Destroy();
                WeaponDamage(trap.hittedActor, 50f, DamageType.Arcane);
            };


            //Watch for monster proximity for the next 12 sec
            /*while (hero.World.Game.Tick < trapTtl)
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
            trap.Destroy();*/

            yield break;
        }
    }

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.BolaShot)]		// Level 4
    public class DemonHunterBolaShot : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            GeneratePrimaryResource(2f);
			
			User.PlayEffectGroup(176854);

            var projectile = new PowerProjectile(User.World, 77569, User.Position, TargetPosition, 1f, 3000, 0.5f, 1f, 6f, 3f);
            projectile.OnHit = () =>
            {               
                SpawnEffect(162007, projectile.getCurrentPosition()); // impact effect
				projectile.Destroy();
                SpawnEffect(153874, projectile.getCurrentPosition());
                WaitSeconds(4.0f);
                SpawnEffect(153727, projectile.getCurrentPosition());
                WeaponDamage(projectile.hittedActor, 50f, DamageType.Arcane);
            };
			
			yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.RapidFire)]		// Level 5
    public class DemonHunterRapidFire : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(30f);

            // HACK: Made up demonic meteor spell, not real RapidFire
            SpawnEffect(185366, TargetPosition);
            yield return WaitSeconds(0.4f);

            IList<Actor> hits = GetTargetsInRange(TargetPosition, 10f);
            WeaponDamage(hits, 10f, DamageType.Fire);
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Vault)]		// Level 6
    public class DemonHunterVault : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UseSecondaryResource(10f);
			
			SpawnProxy(User.Position).PlayEffectGroup(69792);  // alt cast efg: 170231
            yield return WaitSeconds(0.3f);
            User.Teleport(TargetPosition);
            User.PlayEffectGroup(69792);

            yield break;
        }
    }
	
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.Chakram)]		// Level 7
    public class DemonHunterChakram : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(10f);

            var projectile = new PowerProjectile(User.World, 129228, User.Position, TargetPosition, 1f, 3000, 1f, 1f, 0f, 3f);
            projectile.OnHit = () =>
            {
                SpawnEffect(129228, projectile.getCurrentPosition()); // impact effect
                projectile.Destroy();
                WeaponDamage(projectile.hittedActor, 50f, DamageType.Arcane);
            };
			
            yield break;
        }
    }

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.EntanglingShot)]		// Level 8
    public class DemonHunterEntanglingShot : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            GeneratePrimaryResource(4f);
			
			var projectile = new PowerProjectile(User.World, 75678, User.Position, TargetPosition, 1f, 3000, 1f, 1f, 6f, 3f);
            projectile.OnHit = () =>
            {
                SpawnEffect(76223, projectile.getCurrentPosition()); // impact effect
                projectile.Destroy();
                WeaponDamage(projectile.hittedActor, 50f, DamageType.Arcane);
            };

                    //We got a chained actor  //76093
                    if (chainActor != null)
                    {
                        WeaponDamage(projectile.hittedActor, 0f, DamageType.Arcane);
                        User.PlayEffectGroup(76227);
					}

            yield break;
        }

        public object chainActor { get; set; }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.ElementalArrow)]		// Level 9
    public class DemonHunterElementalArrow : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(5f);
        }
    }*/
	
	[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.Grenades)]		// Level 11
    public class DemonHunterGrenades : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            GeneratePrimaryResource(2f);
			
			var projectile = new PowerProjectile(User.World, 88244, User.Position, TargetPosition, 1f, 3000, 1f, 1f, 6f, 3f);
            projectile.OnHit = () =>
            {
                SpawnEffect(153982, projectile.getCurrentPosition()); // impact effect
                projectile.Destroy();
                WeaponDamage(projectile.hittedActor, 10f, DamageType.Arcane);
            };
                
            yield break;
        }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.ShadowPower)]		// Level 12
    public class DemonHunterShadowPower : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UseSecondaryResource(20f);
        }
    }*/

    [ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.FanOfKnives)]		// Level 14
    public class DemonHunterFanOfKinves : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(20f);
			
			StartCooldown(WaitSeconds(10f));

            User.PlayEffectGroup(77547);

            if (CanHitMeleeTarget(Target))
            {
                
                if (Rand.NextDouble() < 0.20)
                    Knockback(Target, 4f);

                WeaponDamage(Target, 50f, DamageType.Physical);
            }

            yield break;
        }
    }
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Companion)]		// Level 15
    public class DemonHunterCompanion : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UseSecondaryResource(10f);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.SpikeTrap)]		// Level 16
    public class DemonHunterSpikeTrap : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            GeneratePrimaryResource(6f);
        }
    }*/

    /*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.Multishot)]		// Level 17
    public class DemonHunterMultishot : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(15f);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.SmokeScreen)]		// Level 18
    public class DemonHunterSmokeScreen : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UseSecondaryResource(15f);
			
			User.PlayEffectGroup(131425);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredGenerators.Strafe)]		// Level 19
    public class DemonHunterStrafe : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(10f);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Sentry)]		// Level 21
    public class DemonHunterSentry : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UseSecondaryResource(10f);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.MarkedForDeath)]		// Level 23
    public class DemonHunterMarkedForDeath : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UseSecondaryResource(6f);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.Discipline.Preparation)]		// Level 25
    public class DemonHunterPreparation : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            StartCooldown(WaitSeconds(120f));
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.ClusterArrow)]		// Level 27
    public class DemonHunterClusterArrow : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(50f);
        }
    }*/
	
	/*[ImplementsPowerSNO(Skills.Skills.DemonHunter.HatredSpenders.RainOfVengeance)]		// Level 29
    public class DemonHunterRainOfVengeance : PowerImplementation
    {
        public override IEnumerable<TickTimer> Run()
        {
            UsePrimaryResource(40f);
			
			StartCooldown(WaitSeconds(30f));
        }
    }*/
}
