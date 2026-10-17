public static class StringExtension
{
    //FNV-1a hash algorithm. se hace el or (^) y luego se multiplica por el prime fnv en cada byte
    public static int ComputerFNV1aHash(this string str)
    {
        uint hash = 2166136261;
        foreach(char c in str)
        {
            hash = (hash ^ c) * 16777619;
        }
        return unchecked((int)hash);
    }
}