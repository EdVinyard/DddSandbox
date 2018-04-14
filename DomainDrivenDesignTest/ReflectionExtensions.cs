﻿using System;

namespace DomainDrivenDesignTest
{
    public static class ReflectionExtensions
    {
        public static bool Implements<TBase>(this Type t) =>
            typeof(TBase).IsAssignableFrom(t);

        public static bool IsDerivedFrom<TBase>(this Type t) =>
            typeof(TBase).IsAssignableFrom(t);
    }
}
