# Online book library
End course task for even faculty numbers



## Structure of the system by entities:

- Book
  - Int BookId
  - String Title *
  - DateTime ReleaseDate
  - Int AuthorId *
  - Int GenreId *
  - String Description

- Writer
  - Int WriterId
  - String FirstName *
  - String LastName
  - String UserName

- Genre
  - Int GenreId
  - String GenreName *



## Restrictions
Required fields are mark with stars. Eligible characters:
* Book.Name – acceptable characters between 1 and 300.
* Book.Discription – acceptable characters between 1 and 500.
* Writer.FirstName – acceptable characters between 1 and 200.
* Writer.LastName – acceptable characters between 1 and 200.
* Genre.GenreName – acceptable characters between 1 and 100.



## For each model have to be created a CRUD of views.



## Filters
Books/Index view need to have:
* Pager
* Search on Title, Genre have to be DropDownList and Writer



## The online library need to have and authentication. Roles and access:
* Admin - Full access
* User - Need to access Books/Index page, and need to have rights to use search panel. But without access to Create, Update, Delete and access to Details.
* Anonymous – not register users. They can see only Books/Index. But without access to Create, Update, Delete and Details



### Information
In the description for controller to view, we will use the following abbreviation.
Example: View Index to controller Books is Books/Index
