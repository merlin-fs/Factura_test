using System;

namespace Game.Core.Units
{
    public interface ISkill: IDisposable
    {
        void Initialize();
        ISkill Clone();
    }
}
