using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ToggleReplayInfo.Configuration
{
    public class PluginConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual bool HideReplayInfo { get; set; } = true;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color TextColor { get; set; } = Color.white;
        public virtual float TextAlpha { get; set; } = 1f;

        public virtual Vector3S Position { get; set; } = Vector3S.Default();

        public virtual float Scale { get; set; } = 1f;

        public virtual Vector3S Rotation { get; set; } = new Vector3S();

        public void SetColor(Color col)
        {
            TextColor = col;
            TextAlpha = col.a;
        }

        public Color GetColor()
        {
            return new Color(TextColor.r, TextColor.g, TextColor.b, TextAlpha);
        }

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