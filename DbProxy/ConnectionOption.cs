namespace DbProxy
{
    public enum ConnectionOption
    {
        FirstOnly = 0b0000,
        Fallback = 0b0001,
        RoundRobin = 0b0010,
        RoundRobinWithFallback = 0b0011
    }
}
