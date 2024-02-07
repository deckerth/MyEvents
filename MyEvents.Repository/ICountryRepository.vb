Imports MyEvents.Models

Namespace Global.MyEvents.Repository

    Public Interface ICountryRepository

        ' Returns all Countrys. 
        Function GetAsync() As Task(Of IEnumerable(Of Country))

        ' Returns all Countrys with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Country))

        ' Returns the Country with the given id. 
        Function GetAsync(id As Guid) As Task(Of Country)

        ' Returns the Country with the given name. 
        Function GetAsyncExact(search As String) As Task(Of Country)

        ' Adds a new Country if it does not exist
        Function Insert(Country As Country) As Task

        ' Deletes the entry the given name. 
        Function DeleteAsyncExact(search As String) As Task

        ' Clears the database
        Function ClearAsync() As Task
    End Interface

End Namespace
