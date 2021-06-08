using IPA.Config.Stores;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ToggleReplayInfo.Configuration
{
    public class PluginConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual bool HideReplayInfo { get; set; } = true;

        public virtual Vector3S Position { get; set; } = Vector3S.Default();

        public virtual float Scale { get; set; } = 1f;

        public virtual Vector3S Rotation { get; set; } = new Vector3S();


        public class Vector3S
        {
            public Vector3S() { }
            public static Vector3S Default()
            {
                return new Vector3S()
                {
                    X = 0,
                    Y = 4,
                    Z = 12
                };
            }

            public virtual float X { get; set; } = 0;
            public virtual float Y { get; set; } = 0;
            public virtual float Z { get; set; } = 0;

            public Vector3 ToVector3()
            {
                return new Vector3(X,Y,Z);
            }
        }
    }
}