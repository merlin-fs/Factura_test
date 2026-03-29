using Game.Core.Units;
using VContainer;

namespace Game.Client.Units
{
    public abstract class BaseUnit : Unit
    {
        protected BaseUnit(UnitConfig config, IObjectResolver container) : base(config)
        {
            foreach (var skill in Skills.All)
                container.Inject(skill);
            foreach (var stat in Stats.All)
                container.Inject(stat);
        }
    }
}

