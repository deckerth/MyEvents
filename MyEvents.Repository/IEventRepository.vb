Imports MyEvents.Models

Namespace Global.MyEvents.Repository
    Public Interface IEventRepository

        Enum UpsertResult
            added
            updated
            skipped
        End Enum

        ' Returns all Performances. 
        Function GetAsync() As Task(Of IEnumerable(Of Performance))

        ' Returns all Performances with a data field matching the start of the given string. 
        Function GetAsync(search As String) As Task(Of IEnumerable(Of Performance))

        ' Returns the Performance with the given id. 
        Function GetAsync(id As Guid) As Task(Of Performance)

        ' Returns a Performance with the given work, composer, and performance date. 
        Function GetAsync(work As String, composer As String, performedAt As String) As Task(Of Performance)

        ' Adds a new Performance if it does not exist, updates the 
        ' existing Performance otherwise.
        Function Upsert(Performance As Performance) As Task(Of UpsertResult)

        ' Deletes a Performance.
        Function DeleteAsync(id As Guid) As Task

    End Interface

End Namespace
