
namespace MiniAuth.Application.Abstractions
{
    public interface ITokenHasher
    {
        string Hash(string token);
    }
}
