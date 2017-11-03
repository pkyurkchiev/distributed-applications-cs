#Online book library


#
## Structure of the system by enties:

* Book
Int BookId
String Title *
DateTime ReleaseDate
Int AuthorId *
Int GenreId *
String Description

* Writer
Int AuthorId
String FirstName *
String LastName
String UserName

* Genre
Int GenreId
String GenreName *


#
## Restrictions
Requered fields are mark with stars.
Eligible Characters:
Book.Name – acceptable characters between 1 and 300.
Book.Discription – acceptable characters between 1 and 500.
Author.FirstName – acceptable characters between 1 and 200.
Author.LastName – acceptable characters between 1 and 200.
Genre.GenreName – acceptable characters between 1 and 100.


#
## For each model have to be created a CRUD of views.


#
## Filters
Books/Index view need to have:
Pager
Serach on Title, Genre have to be DropDownList and Writer


#
## The online library need to have and authentification.
Admin - Full access
User - Need to access Books/Index page, and need to have rights to use search panel. But without access to Create, Update, Delete.
Anonymous – not register users. They can see only Books/Index.


#
###Informations
In the description for controller to view, we will use the following abbreviation:
Example
View Index to contorller Books is Books/Index