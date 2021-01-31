Imports MyEvents.Models

Namespace Global.MyEvents.Repository

    Public Interface ISoloistRepository

        ' Returns all Soloists. 
        Function GetAsync() As Task(Of IEnumerable(Of Soloist))

        ' Returns all Soloists with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Soloist))

        ' Returns the Soloist with the given id. 
        Function GetAsync(id As Guid) As Task(Of Soloist)

        ' Returns the Soloist with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Soloist)

        ' Adds a new Soloist if it does not exist
        Function Insert(Soloist As Soloist) As Task(Of Boolean)

        ' Clears the database
        Function ClearAsync() As Task

    End Interface

End Namespace
