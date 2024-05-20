using Chinook.Models;

namespace Chinook.ClientModels
{
    public class ArtistDto
    {
        public long ArtistId { get; set; }
        public string? Name { get; set; }
        //public int AlbumsCount { get; set; }
        public List<AlbumDto> Albums { get; set; } = [];
    }
}
