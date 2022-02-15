using System;
using System.Collections.Generic;
using System.Text;

namespace PersistedSortedList
{
    public class Constants
    {
        public static int NodeLength = 2 + 8 * BranchingFactor + 8 * (BranchingFactor + 1) + 2 * BranchingFactor;
        public const int BranchingFactor = 3;
    }
}
