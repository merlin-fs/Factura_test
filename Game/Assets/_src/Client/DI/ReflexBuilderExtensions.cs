using System;
using Reflex.Core;
using Reflex.Enums;

namespace Game.Client.DI
{
    public static class ReflexBuilderExtensions
    {
        public static ContainerBuilder RegisterType<TImplementation>(
            this ContainerBuilder builder,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return builder.RegisterType(typeof(TImplementation), lifetime, resolution);
        }

        /// <summary>
        /// Sugar only.
        /// Регистрирует concrete implementation, а контракт TContract
        /// Reflex резолвит через implemented interface/base type.
        /// </summary>
        public static ContainerBuilder RegisterType<TContract, TImplementation>(
            this ContainerBuilder builder,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
            where TImplementation : TContract
        {
            return builder.RegisterType(typeof(TImplementation), new []{typeof(TContract)}, lifetime, resolution);
        }

        public static ContainerBuilder RegisterValue<TValue>(
            this ContainerBuilder builder,
            TValue value)
        {
            return builder.RegisterValue(value);
        }
    }
}