using Microsoft.AspNetCore.Http;

namespace TinyChat.Helpers
{
    public interface IFileValidator
    {
        bool IsValid(IFormFile file);
    }
}
