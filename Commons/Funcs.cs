using System;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using Nautilus.Utility;
using UnityEngine;

namespace AshFox.Subnautica
{
    public static class Funcs
    {
        public static readonly Func<float, bool> CHECK_POSITIVE_FLOAT = (value) => value > 0.0f;
        public static readonly Func<float, bool> CHECK_POSITIVE_OR_ZERO_FLOAT = (value) =>
            value >= 0.0f;
    }
}
