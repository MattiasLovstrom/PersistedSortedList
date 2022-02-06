namespace PersistedSortedList.Tests
{
    public enum FreeType
    {
        // node was freed (available for GC, not stored in freelist)        
        ftFreeListFull,

        // node was stored in the freelist for later use        
        ftStored,

        // node was ignored by COW, since it's owned by another one        
        ftNotOwned
    }
}