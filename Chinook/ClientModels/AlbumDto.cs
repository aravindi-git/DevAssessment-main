namespace Chinook.ClientModels
{
    public class AlbumDto
    {
        public long AlbumId { get; set; }
        public string Title { get; set; } = null!;
        public long ArtistId { get; set; }
        //public List<TrackListItemDto> Tracks { get; set; }
    }
}
