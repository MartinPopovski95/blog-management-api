namespace BlogManagementApi.Services.Abstractions
{
    public interface IMediaService
    {
        Task<string> UploadAsync(Stream stream, string fileName);
    }
}
