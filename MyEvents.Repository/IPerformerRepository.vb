Imports MyEvents.Models

Namespace Global.MyEvents.Repository

    Public Interface IPerformerRepository

        ' Returns all Performers. 
        Function GetAsync() As Task(Of IEnumerable(Of Performer))

        ' Returns all Performers with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Performer))

        ' Returns the Performer with the given id. 
        Function GetAsync(id As Guid) As Task(Of Performer)

        ' Returns the Performer with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Performer)

        ' Adds a new Performer if it does not exist
        Function Insert(Performer As Performer) As Task(Of Boolean)

        ' Clears the database
        Function ClearAsync() As Task

    End Interface

End Namespace
