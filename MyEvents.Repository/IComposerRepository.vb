Imports MyEvents.Models

Namespace Global.MyEvents.Repository

    Public Interface IComposerRepository

        ' Returns all Composers. 
        Function GetAsync() As Task(Of IEnumerable(Of Composer))

        ' Returns all Composers with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Composer))

        ' Returns the Composer with the given id. 
        Function GetAsync(id As Guid) As Task(Of Composer)

        ' Returns the Composer with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Composer)

        ' Adds a new Composer if it does not exist
        Function Insert(Composer As Composer) As Task

        ' Deletes the entry the given name. 
        Function DeleteAsyncExact(search As String) As Task

        ' Clears the database
        Function ClearAsync() As Task

    End Interface


End Namespace
