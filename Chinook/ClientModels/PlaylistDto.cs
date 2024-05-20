namespace Chinook.ClientModels;

public class PlaylistDto
{
    public string Name { get; set; }
    public List<PlaylistTrack> Tracks { get; set; }
}