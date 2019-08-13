using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IntoTheBlaze {

    // http://www.femalifesafety.org/types-of-extinguishers.html
    public class ExtinguishingAgent {

        public readonly int PartVariant;

        private readonly HashSet<FireType> efficientAgainst, against;

        public bool IsConductive { get; }

        public bool Extinguishes(FireType type) => efficientAgainst.Contains(type) || against.Contains(type);

        public bool IsEffectiveAgainst(FireType type) => efficientAgainst.Contains(type);

        public Color PartColor { get; }

        public ExtinguishingAgent(Color partColor, int partVariant, FireType[] efficientAgainst = null, FireType[] against = null, bool conductive = false) {
            this.efficientAgainst = new HashSet<FireType>();
            if (this.efficientAgainst != null)
                foreach (var fireType in this.efficientAgainst)
                    this.efficientAgainst.Add(fireType);
            this.against = new HashSet<FireType>();
            if (against != null)
                foreach (var fireType in against)
                    this.against.Add(fireType);
            PartColor = partColor;
            PartVariant = partVariant;
            IsConductive = conductive;
        }

        public static ExtinguishingAgent
            Water = new ExtinguishingAgent(new Color(64, 160, 255), 1, new[] { FireType.A, FireType.C }, conductive: true),
            Foam = new ExtinguishingAgent(new Color(255, 224, 255), 0, new[] { FireType.A, FireType.B, FireType.C }, conductive: true),
            CO2 = new ExtinguishingAgent(Color.White, 1, new[] { FireType.B, FireType.C }, new[] { FireType.A }),
            DryPowder = new ExtinguishingAgent(new Color(192, 255, 128), 0, new[] { FireType.D }),
            WetChemical = new ExtinguishingAgent(new Color(255, 255, 192), 1, new[] { FireType.K }, new[] { FireType.A }),
            WaterMist = new ExtinguishingAgent(new Color(128, 192, 255), 0, new[] { FireType.A, FireType.C });
    }
}
