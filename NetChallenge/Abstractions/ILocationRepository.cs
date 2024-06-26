﻿using NetChallenge.Domain;

namespace NetChallenge.Abstractions
{
    public interface ILocationRepository : IRepository<Location>
    {
        bool LocationAlreadyExists(string name);
    }
}