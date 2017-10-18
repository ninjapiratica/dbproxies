namespace DbProxy
{
    public enum ConnectionOption
    {
        FirstOnly = 0,
        Fallback = 1,
        RoundRobin = 2,
        RoundRobinWithFallback = 3
    }
}
