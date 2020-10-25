# Online electronic game catalog
End course task for odd faculty numbers



## Structure of the system by entities:

- Game
  - Int GameId
  - String Name *
  - String ReleaseYear *
  - Int GenreId
  - Int RatingId

- Genre
  - Int GenreId
  - String GenreName *
  - String Description

- Rating
  - Int RatingId
  - String RatingValue *
  - String Description *



## Restrictions
Required fields are mark with stars. Eligible characters:
* Game.Name – acceptable characters between 1 and 200.
* Genre.GenreName – acceptable characters between 1 and 100.
* Rating.RatingValue – acceptable characters between 1 and 100.
* Rating.Description – acceptable characters between 1 and 400.



## For each model have to be created a CRUD of views.



## Filters
Games/Index view need to have:
* Pager
* Sort column on Name, Genre and Rating.



## The online library need to have and authentication. Roles and access:
* Admin - Full access
* User - Need to access Games/Index page, and need to have rights to use search panel. But without access to Create, Update, Delete and access to Details.
* Anonymous – not register users. They can see only Games/Index and can use search panel. But without access to Create, Update, Delete and Details.



### Information
In the description for controller to view, we will use the following abbreviation.
Example: View Index to controller Games is Games/Index
