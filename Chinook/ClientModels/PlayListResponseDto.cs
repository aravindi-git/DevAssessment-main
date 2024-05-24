namespace Chinook.ClientModels
{
    public class PlayListResponseDto
    {

        public long PlaylistId { get; set; }
        public bool SuccessfullyAdded { get; set; }
        public string Message { get; set; }
        public bool IsNewPlaylist { get; set; }
    }
}
