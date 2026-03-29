using Game.Core.Units;

namespace Game.Core.Services
{
    public readonly struct DamageRequest
    {
        public Unit Source { get; }
        public Unit Target { get; }
        public int  Amount { get; }

        public DamageRequest(Unit source, Unit target, int amount)
        {
            Source = source;
            Target = target;
            Amount = amount;
        }
    }
}

