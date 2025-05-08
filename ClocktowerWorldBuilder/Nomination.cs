namespace ClocktowerWorldBuilder;

public record Nomination(
    int Day,
    int Target,
    int Nominator,
    bool VirginDeath,
    bool Success
);

