using Game.Core.Units;

namespace Game.Core.Services
{
    public interface IHitHandler
    {
        void Handle(Unit source, Unit target);
    }
}