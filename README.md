
# Code explanation by Aravindi Amarasinghe

1. I have moved data fetching functinality to a spetarate section, by adding a new folder called Services. 
   Cretaed seperate service classes and their corresponding interfaces for Atrist, Album and Playlist entities.

2. Registerd the services in the dependecy injection container in the Program.cs file. 

3. Did some changes in the ChinookContext.cs file, in the Playlist entity to make adding playlists feature possible. 
	change 1 : Changed ValueGeneratedNever to ValueGeneratedOnAdd on the PlaylistId field to make the playlistId to generate automatically when creating a new one.
	change 2 : Updated the Many to Many relationship initialization between the playlists and tracks. So we can add tracks to playlists and create new playlists with tracks. 

4. Initiated an AutoMapper profile to map the entity models to client models, define maps and register it on the dependeny injection container in Program cs file. 
   Used the Automapper when mapping the models to client models in the service classes. 

5. Implemented the functionality to display the albums count of the Artists on the home page. 

6. Implemented the functionality to make a track as a favorite one on the Artist page. (Here, if the favorite playlist is alteady exists, then add the track to it. Otherwise, 
   Create a new playlist with the name of "My favorite tracks" and then add track to it. )

7. Implemented the functionality to remove tracks from the favorite list. 

8. Developed the functionalities for the dialog box(modal) on the Atrist page to create a new playlist, and add a track to an exisiting playlist. 

9. Listed the exisiting playlists from the database and bind them on the dropdown list in the above mentioned dialog box. 

10. Improve the dialog box by hiding it after saving the chages and resetting the form. 

11. Used Exception handelling on the methods of the services. 

12. Changed the NavMenu page, Added a functionality to fetch the user's own playlists and display them on the nav menu. 

13. Added the routing to the navmenu items so that we can go to playlist page and check the available tracks. 
   (Here, we have to reload the page to view the playlists nav items. I could not integrate SectionOutlets to make it dynamic)

14. Implemented the functionality to remove tracks from the user's  playlists. 

15. Removed unnessary code snippets.

16. Changed the class name of the star icon for favorite tracks on the Arist page. 

17. Added "My favorite tracks" as a constant, so we can use it when needed, without initializing the same string again and again. 

