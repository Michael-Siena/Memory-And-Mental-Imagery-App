public interface ITimeoutable
{
    float Duration { get; } // in seconds
    float TimeLeft { get; }

    bool HasTimedOut();
}