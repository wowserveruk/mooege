using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooege.Net.GS.Message.Definitions.Effect;

namespace Mooege.Core.GS.Actors
{
    public class Effect2
    {
        private Actor actor;

        public Effect2(Actor owner)
        {
            this.actor = owner;
        }

        public void addEffect2(int effectSNO, uint targetId)
        {
            this.actor.World.BroadcastIfRevealed(new EffectGroupACDToACDMessage()
            {
                Id = 170,
                fromActorID = this.actor.DynamicID,
                toActorID = targetId,
                effectSNO = effectSNO

            }, this.actor);
        }

        public void addEffect2(int effectSNO)
        {
            this.actor.World.BroadcastIfRevealed(new PlayEffectMessage()
            {
                Id = 122,
                ActorId = this.actor.DynamicID,
                OptionalParameter = effectSNO,
                //Effect = EffectId.PlayEffectGroup

            }, this.actor);
        }

        public Effect2 PlayEffectGroup { get; set; }
    }
}
