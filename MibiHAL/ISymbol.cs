﻿namespace MibiHAL
{
    public interface ISymbol
    {
        string Value { get; }
        bool Terminal { get; }
    }
}