Imports MyEvents.Models

Namespace Global.MyEvents.Repository

    Public Interface IVenueRepository

        ' Returns all Venues. 
        Function GetAsync() As Task(Of IEnumerable(Of Venue))

        ' Returns all Venues with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Venue))

        ' Returns the Venue with the given id. 
        Function GetAsync(id As Guid) As Task(Of Venue)

        ' Returns the Venue with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Venue)

        ' Returns the Venue with the given name. 
        Function GetAsyncExactWithTracking(search As String) As Task(Of Venue)

        ' Adds a new Venue if it does not exist
        Function Insert(Venue As Venue) As Task

        ' Deletes the entry the given name. 
        Function DeleteAsyncExact(search As String) As Task

        ' Clears the database
        Function ClearAsync() As Task
    End Interface

End Namespace
