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
using Mooege.Common.Helpers;
using Mooege.Core.GS.Map;
using Mooege.Net.GS.Message;
using Mooege.Net.GS.Message.Definitions.Tick;
using Mooege.Net.GS.Message.Definitions.World;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.Animation;
using Mooege.Net.GS.Message.Definitions.Effect;
using Mooege.Net.GS.Message.Definitions.Misc;
using Mooege.Core.GS.Actors;
using Mooege.Core.GS.Common.Types.Math;

namespace Mooege.Core.GS.Powers
{
    public class PowerProxy : Actor
    {
        public override ActorType ActorType { get { return ActorType.Proxy; } }

        // TODO: Setter needs to update world. Also, this is probably an ACD field. /komiga
        public int AnimationSNO { get; set; }

        public PowerProxy(World world, int actorSNO, Vector3D position)
            : base(world, world.NewActorID)
        {
            this.ActorSNO = actorSNO;
            // FIXME: This is hardcoded crap
            this.Field2 = 0x8;
            this.Field3 = 0x0;
            this.Scale = 1f;
            this.Position.Set(position);
            this.RotationAmount = 0f;
            this.RotationAxis.X = 0f; this.RotationAxis.Y = 0f; this.RotationAxis.Z = 1f;
            this.GBHandle.Type = (int)GBHandleType.Monster; this.GBHandle.GBID = 1;
            this.Field7 = 0x00000001;
            this.Field8 = this.ActorSNO;
            this.Field10 = 0x0;
            this.Field11 = 0x0;
            this.Field12 = 0x0;
            this.Field13 = 0x0;
            this.AnimationSNO = 0x11150;

            this.Attributes[GameAttribute.Untargetable] = false;
            this.Attributes[GameAttribute.Uninterruptible] = true;
            
            this.World.Enter(this); // Enter only once all fields have been initialized to prevent a run condition
        }
        
        public override bool Reveal(Mooege.Core.GS.Players.Player player)
        {
            if (!base.Reveal(player))
                return false;

            /* Dont know what this does
            player.InGameClient.SendMessage(new ANNDataMessage(Opcodes.ANNDataMessage24)
            {
                ActorID = this.DynamicID
            });
            */

            player.InGameClient.SendMessage(new SetIdleAnimationMessage
            {
                ActorID = this.DynamicID,
                AnimationSNO = this.AnimationSNO
            });

            player.InGameClient.SendMessage(new EndOfTickMessage()
            {
                Field0 = player.InGameClient.Game.Tick,
                Field1 = player.InGameClient.Game.Tick + 20
            });

            return true;
        }

        public int ActorSNO { get; set; }
    }
}
