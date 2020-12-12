Imports MyEvents.Models

Namespace Global.MyEvents.Repository

    Public Interface IDirectorRepository

        ' Returns all Directors. 
        Function GetAsync() As Task(Of IEnumerable(Of Director))

        ' Returns all Directors with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Director))

        ' Returns the Director with the given id. 
        Function GetAsync(id As Guid) As Task(Of Director)

        ' Returns the Director with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Director)

        ' Adds a new Director if it does not exist
        Function Insert(Director As Director) As Task

        ' Clears the database
        Function ClearAsync() As Task

    End Interface

End Namespace
