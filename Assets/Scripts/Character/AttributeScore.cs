using System;

namespace MythRealFFSV2.Character
{
    /// <summary>
    /// Represents a single attribute score with its calculated modifier
    /// </summary>
    [Serializable]
    public class AttributeScore
    {
        public AttributeType type;
        public int baseValue;
        public int temporaryBonus;

        public int Value => baseValue + temporaryBonus;

        /// <summary>
        /// Calculate the modifier based on the attribute score
        /// According to the rulebook:
        /// 19-20: +5, 17-18: +4, 15-16: +3, 13-14: +2, 11-12: +1,
        /// 9-10: 0, 7-8: -1, 5-6: -2, 3-4: -3, 1-2: -4
        /// </summary>
        public int Modifier
        {
            get
            {
                int val = Value;
                if (val >= 19) return 5;
                if (val >= 17) return 4;
                if (val >= 15) return 3;
                if (val >= 13) return 2;
                if (val >= 11) return 1;
                if (val >= 9) return 0;
                if (val >= 7) return -1;
                if (val >= 5) return -2;
                if (val >= 3) return -3;
                return -4;
            }
        }

        public AttributeScore(AttributeType type, int baseValue = 10)
        {
            this.type = type;
            this.baseValue = baseValue;
            this.temporaryBonus = 0;
        }

        public void ApplyBonus(int bonus)
        {
            temporaryBonus += bonus;
        }

        public void ClearTemporaryBonuses()
        {
            temporaryBonus = 0;
        }

        public override string ToString()
        {
            return $"{type}: {Value} ({(Modifier >= 0 ? "+" : "")}{Modifier})";
        }
    }
}
