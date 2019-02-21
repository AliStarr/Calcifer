namespace Booper.Common
{
    /// <summary>
    /// The enum used to specify permission levels. A lower
    /// number means less permissions than a higher number.
    /// </summary>
    public enum AccessLevel
    {
        Blocked,        // 0
        User,           // 1
        ServerMod,      // 2
        ServerAdmin,    // 3
        ServerOwner,    // 4
        BotOwner        // 5
    }
}
