using System;

public static class Globals
{
    public enum Role
    {
        Attacker,
        Defender
    };
    public static int PeerId;
    public static Role CurrentRole;
}