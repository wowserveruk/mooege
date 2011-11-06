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
using System.Reflection;
using Mooege.Common;
using Mooege.Core.GS.Actors;
using Mooege.Net.GS.Message.Definitions.World;
using Mooege.Net.GS.Message.Fields;
using Mooege.Common.Helpers;
using Mooege.Core.GS.Ticker.Helpers;
using Mooege.Core.GS.Common.Types.Math;

namespace Mooege.Core.GS.Powers
{
    public abstract class PowerImplementation2
    {
        private static Dictionary<int, Type> _implementations = new Dictionary<int, Type>();

        public static PowerImplementation2 ImplementationForPowerSNO(int powerSNO)
        {
            if (_implementations.ContainsKey(powerSNO))
            {
                return (PowerImplementation2)Activator.CreateInstance(_implementations[powerSNO]);
            }

            return null;
        }

        static PowerImplementation2()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.IsSubclassOf(typeof(PowerImplementation2)))
                {
                    var attributes = (ImplementsPowerSNO[])type.GetCustomAttributes(typeof(ImplementsPowerSNO), true);
                    foreach (var powerAttribute in attributes)
                    {
                        _implementations[powerAttribute.PowerSNO] = type;
                    }
                }
            }
        }

        public abstract IEnumerable<int> Run(Actor player, Actor target, Vector3D mousePosition, TargetMessage msg);
    }
	
	public abstract class PowerImplementation : PowerContextHelper
    {
        public static readonly Logger Logger = LogManager.CreateLogger();

        // Called to start executing a power
        // Yields timers that signify when to continue execution.
        public abstract IEnumerable<TickTimer> Run();

        // token instance that can be yielded by Run() to indicate the power manager should stop
        // running a power implementation.
        public static TickTimer StopExecution = new TickTimer(null, 0);
    }
}
