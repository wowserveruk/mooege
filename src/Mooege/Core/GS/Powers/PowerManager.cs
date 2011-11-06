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

using System.Collections.Generic;
using Mooege.Common;
using Mooege.Common.Helpers;
using Mooege.Core.GS.Actors;
using Mooege.Core.GS.Common.Types.Math;
using Mooege.Core.GS.Players;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.World;
using System.Linq;
using Mooege.Core.GS.Ticker.Helpers;
using System;
using Mooege.Net.GS.Message.Fields;

namespace Mooege.Core.GS.Powers
{
    public class PowerManager2
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        
        protected Actor _owner;

        //Master list of delayed action
        private Dictionary<IEnumerator<int>, DateTime> waitingImplementation = new Dictionary<IEnumerator<int>,DateTime>();
        
        //Used by frenzy skill impl.
        public int frenzyStack = 0;

        //Min range for melee hit
        protected float meleeRange = 12f;
        
        public PowerManager2(Actor owner)
        {
            //Register the powermanager user
            this._owner = owner;
        }

        public void Run(uint targetId, int powerSNO, Vector3D mousePosition, TargetMessage msg)
        {
            //Try to retreive targeted actor
            Actor target = this._owner.World.GetActor(targetId);

            if (target != null)
                mousePosition = target.Position;

            if (PowerImplementation2.ImplementationForPowerSNO(powerSNO) == null)
            {
                Console.Write("Power not implemented : " + powerSNO);
                return;
            }
            
            //Init the required implementation
            IEnumerable<int> powerExe = PowerImplementation2.ImplementationForPowerSNO(powerSNO).Run(this._owner, target, mousePosition, msg);
            var powerEnum = powerExe.GetEnumerator();
    
            //Run the implementation
            if (powerEnum.MoveNext())
            {
                //If the implementation ask for a delay, put it on the waiting list
                this.waitingImplementation.Add(powerEnum, DateTime.Now.AddMilliseconds(powerEnum.Current));
            }
        }

        public void UpdateWaitingImplementation()
        {
            Dictionary<IEnumerator<int>, DateTime> new_waitingpower = new Dictionary<IEnumerator<int>, DateTime>();

            foreach (KeyValuePair<IEnumerator<int>, DateTime> power in this.waitingImplementation)
            {
                //Check if it's time to run the Waiting Implementation
                if (power.Value < DateTime.Now)
                {
                    //Check if there are more delayed action for this power, if so, put the power on the waiting list again
                    if (power.Key.MoveNext())
                        new_waitingpower.Add(power.Key, DateTime.Now.AddMilliseconds(power.Key.Current));
                }
                else
                {
                    new_waitingpower.Add(power.Key, power.Value);
                }
            }

            //Assign new power list
            this.waitingImplementation = new_waitingpower;
        }

        //GameTick Update
        public void Update()
        {
            this.UpdateWaitingImplementation();
        }
    }
	
	public class PowerManager
    {
        static readonly Logger Logger = LogManager.CreateLogger();

        // list of all actively channeled powers
        private List<ChanneledPowerImplementation> _channeledPowers = new List<ChanneledPowerImplementation>();

        // list of all waiting to execute powers
        private class WaitingPower
        {
            public IEnumerator<TickTimer> PowerEnumerator;
            public PowerImplementation Implementation;
        }
        private List<WaitingPower> _waitingPowers = new List<WaitingPower>();
        
        public PowerManager()
        {
        }

        public void Update()
        {
            UpdateWaitingPowers();
        }

        public bool UsePower(Actor user, int powerSNO, uint targetId = uint.MaxValue, Vector3D targetPosition = null,
                             TargetMessage message = null)
        {
            Actor target;

            float targetZ = 0f; // Z value of targetPosition, regardless if a targetId has been specified.
            if (targetPosition != null)
                targetZ = targetPosition.Z;

            if (targetId == uint.MaxValue)
            {
                target = null;
                if (targetPosition == null)
                    targetPosition = new Vector3D(0, 0, 0);
            }
            else if (user.World.GetActorByDynamicId(targetId) != null)
            {
                target = user.World.GetActorByDynamicId(targetId);

                if (targetPosition == null)
                    targetZ = target.Position.Z;

                targetPosition = target.Position;
            }
            else
            {
                return false;
            }
            
            #region Monster spawn HACK
            // HACK: intercept hotbar skill 1 to always spawn test mobs.
            if (user is Player && powerSNO == (user as Player).SkillSet.HotBarSkills[4].SNOSkill)
            {
                // number of monsters to spawn
                int spawn_count = 10;

                // list of actorSNO values to pick from when spawning
                int[] actorSNO_values = { 4282, 3893, 6652, 5428, 5346, 6024, 5393, 5467 };
                int actorSNO = actorSNO_values[RandomHelper.Next(actorSNO_values.Length - 1)];
                //Logger.Debug("PowerManager spawning sno {0}", actorSNO);

                for (int n = 0; n < spawn_count; ++n)
                {
                    Vector3D position;

                    if (targetPosition.X == 0f)
                    {
                        position = new Vector3D(user.Position);
                        if ((n % 2) == 0)
                        {
                            position.X += (float)(RandomHelper.NextDouble() * 20);
                            position.Y += (float)(RandomHelper.NextDouble() * 20);
                        }
                        else
                        {
                            position.X -= (float)(RandomHelper.NextDouble() * 20);
                            position.Y -= (float)(RandomHelper.NextDouble() * 20);
                        }
                    }
                    else
                    {
                        position = new Vector3D(targetPosition);
                        position.X += (float)(RandomHelper.NextDouble() - 0.5) * 20;
                        position.Y += (float)(RandomHelper.NextDouble() - 0.5) * 20;
                        position.Z = user.Position.Z;
                    }

                    Monster mon = new Monster(user.World, actorSNO, null);
                    mon.Position = position;
                    mon.Scale = 1.35f;
                    mon.Attributes[GameAttribute.Hitpoints_Max_Total] = 50f;
                    mon.Attributes[GameAttribute.Hitpoints_Max] = 50f;
                    mon.Attributes[GameAttribute.Hitpoints_Total_From_Level] = 0f;
                    mon.Attributes[GameAttribute.Hitpoints_Cur] = 50f;
                    user.World.Enter(mon);
                }

                return true;
            }
            #endregion

            // find and run a power implementation
            var implementation = PowerLoader.CreateImplementationForPowerSNO(powerSNO);
            if (implementation != null)
            {
                // replace implementation with existing channel instance if one exists
                if (implementation is ChanneledPowerImplementation)
                {
                    var chanpow = _FindChannelingPower(user, powerSNO);
                    if (chanpow != null)
                        implementation = chanpow;
                }

                // copy in context params
                implementation.PowerManager = this;
                implementation.PowerSNO = powerSNO;
                implementation.User = user;
                implementation.Target = target;
                implementation.World = user.World;
                implementation.TargetPosition = targetPosition;
                implementation.TargetZ = targetZ;
                implementation.Message = message;

                // process channeled power events
                var channeledPower = implementation as ChanneledPowerImplementation;
                if (channeledPower != null)
                {
                    if (channeledPower.ChannelOpen)
                    {
                        channeledPower.OnChannelUpdated();
                    }
                    else
                    {
                        channeledPower.OnChannelOpen();
                        channeledPower.ChannelOpen = true;
                        _channeledPowers.Add(channeledPower);
                    }
                }
                
                var powerEnum = implementation.Run().GetEnumerator();
                // actual power will first run here, if it yielded a timer process it in the waiting list
                if (powerEnum.MoveNext() && powerEnum.Current != PowerImplementation.StopExecution)
                {
                    _waitingPowers.Add(new WaitingPower
                    {
                        PowerEnumerator = powerEnum,
                        Implementation = implementation
                    });
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateWaitingPowers()
        {
            // process all powers, removing from the list the ones that expire
            _waitingPowers.RemoveAll((wait) =>
            {
                if (wait.PowerEnumerator.Current.TimedOut)
                {
                    if (wait.PowerEnumerator.MoveNext())
                        return wait.PowerEnumerator.Current == PowerImplementation.StopExecution;
                    else
                        return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public void CancelChanneledPower(Actor user, int powerSNO)
        {
            var channeledPower = _FindChannelingPower(user, powerSNO);
            if (channeledPower != null)
            {
                channeledPower.OnChannelClose();
                channeledPower.ChannelOpen = false;
                _channeledPowers.Remove(channeledPower);
            }
        }

        private ChanneledPowerImplementation _FindChannelingPower(Actor user, int powerSNOId)
        {
            return _channeledPowers.FirstOrDefault(impl => impl.User == user &&
                                                           impl.PowerSNO == powerSNOId &&
                                                           impl.ChannelOpen);
        }
    }
}
