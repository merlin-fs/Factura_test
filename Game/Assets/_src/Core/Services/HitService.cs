using Game.Core.Units;

namespace Game.Core.Services
{
    public sealed class HitService
    {
        private readonly ITargetsProvider _targetsProvider;
        private readonly Unit[] _targetsBuffer = new Unit[16];

        public HitService(ITargetsProvider targetsProvider)
        {
            _targetsProvider = targetsProvider;
        }

        public int Process(in HitQuery query, IHitHandler handler)
        {
            var count = _targetsProvider.Collect(query, _targetsBuffer);

            for (var i = 0; i < count; i++)
            {
                handler.Handle(query.Source, _targetsBuffer[i]);
                _targetsBuffer[i] = null;
            }

            return count;
        }

        public bool ProcessFirst(in HitQuery query, IHitHandler handler)
        {
            var count = _targetsProvider.Collect(query, _targetsBuffer);
            if (count <= 0)
                return false;

            handler.Handle(query.Source, _targetsBuffer[0]);
            _targetsBuffer[0] = null;

            for (int i = 1; i < count; i++)
                _targetsBuffer[i] = null;

            return true;
        }
    }
}