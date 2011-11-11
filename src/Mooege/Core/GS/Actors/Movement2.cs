using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Net.GS.Message.Fields;
using Mooege.Net.GS.Message.Definitions.Actor;
using Mooege.Net.GS.Message.Definitions.ACD;
using Mooege.Core.GS.Common.Types.Math;

namespace Mooege.Core.GS.Actors
{
    public class Movement2
    {
        private Actor actor;

        public Movement2(Actor actor)
        {
            this.actor = actor;
        }

        public void translateNormal(Vector3D finalPosition, int animTag = -1, float speed = 1f, float angle = -1)
        {
            this.actor.World.BroadcastIfRevealed(new NotifyActorMovementMessage() 
            {
                Id = 110,
                //ActorId = this.actor.DynamicID,
                Position = finalPosition,
                Field3 = false,
                Speed = speed,
                AnimationTag = animTag,
                Angle = angle

            }, this.actor);

            //Set actor new position
            this.actor.Position.Set(finalPosition);
        }

        public void translateSpiral(Vector3D finalPosition, int animTag = -1)
        {
            //FIXME : PARAM NEED TO BE FIGURED OUT
            this.actor.World.BroadcastIfRevealed(new ACDTranslateDetPathSpiralMessage()
            {
                Id = 117,
                Field0 = (int)this.actor.DynamicID,
                Field1 = this.actor.Position,
                Field2 = finalPosition,
                Field3 = 1,
                Field4 = 1,
                Field5 = 1,
                Field6 = new DPathSinData() 
                {
                    Field0 = 40,
                    Field1 = 1,
                    Field2 = 1,
                    Field3 = 1,
                    Field4 = 1,
                    Field5 = 1
                }

            }, this.actor);

            //Set actor new position
            this.actor.Position.Set(finalPosition);
        }

        public void translateArc(Vector3D targetPosition, int translateAnimation, float arcHeight, float fallOff = -0.1f) 
        {
            //Calculate velocity vector
            Vector3D delta = new Vector3D(targetPosition.X - this.actor.Position.X, targetPosition.Y - this.actor.Position.Y,
                                          targetPosition.Z - this.actor.Position.Z);

            float delta_length = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
            Vector3D delta_normal = new Vector3D(delta.X / delta_length, delta.Y / delta_length, delta.Z / delta_length);
            float unitsMovedPerTick = 30f;
            Vector3D ramp = new Vector3D(delta_normal.X * (delta_length / unitsMovedPerTick),
                                         delta_normal.Y * (delta_length / unitsMovedPerTick),
                                         arcHeight);

            this.actor.World.BroadcastIfRevealed(new ACDTranslateArcMessage()
            {
                Id = 114,
                Field0 = this.actor.DynamicID,
                Field1 = this.actor.Position,
                Field2 = ramp,
                Field3 = 0, 
                FlyingAnimationTagID = translateAnimation, // used for male barb leap
                LandingAnimationTagID = -1,
                Field6 = fallOff, // leap falloff
                Field7 = -1,
                Field8 = 0

            }, this.actor);

            //Set actor position at the end of the arc
            this.actor.Position.Set(targetPosition);
        }
    }
}
