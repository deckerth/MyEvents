Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider
Imports MyEvents.Models

Namespace Global.MyEvents.Repository.Sql

    Public Class SqlDirectorRepository
        Implements IDirectorRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Director)) Implements IDirectorRepository.GetAsync
            Return Await _db.Directors.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Director)) Implements IDirectorRepository.GetAsync

            Return Await _db.Directors.AsNoTracking().Where(Function(x As Director) x.Name.ToLowerInvariant.Contains(search.ToLowerInvariant)).ToListAsync()

            'Dim parameters As String() = search.Split(" ")
            'Return Await _db.Directors.Where(
            'Function(x As Director) parameters.Any(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
            '        ).OrderByDescending(
            'Function(x As Director) parameters.Count(Function(y As String) x.Name.StartsWith(y, comparisonType:=StringComparison.InvariantCultureIgnoreCase))
            '       ).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Director) Implements IDirectorRepository.GetAsyncExact
            Return Await _db.Directors.AsNoTracking().FirstOrDefaultAsync(Function(x As Director) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Director) Implements IDirectorRepository.GetAsync
            Return Await _db.Directors.AsNoTracking().FirstOrDefaultAsync(Function(x As Director) x.Id = id)
        End Function

        Public Async Function Insert(Director As Director) As Task Implements IDirectorRepository.Insert
            If Await GetAsyncExact(Director.Name) Is Nothing Then
                If Director.Name.Contains("Carriger, Gail") Then
                    Dim x = 0
                End If
                Await _db.Directors.AddAsync(Director)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetDirectors(Directors As List(Of Director)) As Task
            Await ClearAsync()
            _db.Directors.AddRange(Directors)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddDirectors(Directors As List(Of Director)) As Task
            _db.StartMassUpdate()
            For Each i In Directors
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements IDirectorRepository.ClearAsync
            For Each b In _db.Directors
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsyncExact(search As String) As Task Implements IDirectorRepository.DeleteAsyncExact
            Dim toDelete = Await _db.Directors.AsNoTracking().FirstOrDefaultAsync(Function(x As Director) x.Name = search)
            If toDelete IsNot Nothing Then
                _db.Entry(toDelete).State = EntityState.Deleted
            End If
            Await _db.SaveChangesAsync()
        End Function
    End Class

End Namespace
