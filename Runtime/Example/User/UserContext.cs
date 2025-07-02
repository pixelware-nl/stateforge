using Stateforge;
using Stateforge.Interfaces;

namespace Example.User;

public class UserContext : IContext
{
    public int Counter { get; set; }
}