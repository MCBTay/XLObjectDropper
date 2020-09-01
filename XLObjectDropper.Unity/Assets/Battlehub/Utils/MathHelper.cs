using UnityEngine;

namespace Battlehub.Utils
{
    public static class MathHelper
    {
        public static float CountOfDigits(float number)
        {
            return (number == 0) ? 1.0f : Mathf.Ceil(Mathf.Log10(Mathf.Abs(number) + 0.5f));
        }
    }
}

