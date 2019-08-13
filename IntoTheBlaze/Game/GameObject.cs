using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntoTheBlaze.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze.Game {

    public class GameObject {

        public string Name { get; }

        public ObjectType Type { get; }

        public Texture2D Texture { get; }

        public FireType FireType { get; }

        public float FireLimit { get; }

        public IShape FireArea { get; }

        public ExtinguishingAgent AgentSupply { get; }

        public int Width { get; }
        public int Height { get; }
        public int ZHeight { get; }

        public Vector2 TextureOrigin { get; }

        public int MinHP { get; }
        public int MaxHP { get; }

        public int Cost { get; }

        public bool Solid { get; }

        public ReadOnlyCollection<IShape> CollisionMasks { get; }

        public GameObject(
            string name,
            ObjectType type,
            FireType fireType,
            float fireLimit,
            IShape fireArea,
            Texture2D tex,
            int width,
            int height,
            int zHeight,
            int minHp,
            int maxHp,
            ExtinguishingAgent agentSupply,
            IEnumerable<IShape> collisionMasks,
            int cost,
            bool solid
        ) {
            Name = name;
            Texture = tex;
            Width = width;
            Height = height;
            MinHP = minHp;
            MaxHP = maxHp;
            AgentSupply = agentSupply;
            Cost = cost;
            Type = type;
            ZHeight = zHeight;
            FireLimit = fireLimit;
            FireArea = fireArea;
            FireType = fireType;
            Solid = solid;
            CollisionMasks = Array.AsReadOnly(collisionMasks.ToArray());
            TextureOrigin = new Vector2(Width / 2f, Height / 2f);
        }
    }
}
