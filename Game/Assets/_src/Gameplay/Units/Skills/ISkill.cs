using System;

namespace Game.Core.Units
{
    public interface ISkill : IDisposable
    {
        ISkill Clone();
    }
}
