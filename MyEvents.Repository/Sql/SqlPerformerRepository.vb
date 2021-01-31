Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider
Imports MyEvents.Models

Namespace Global.MyEvents.Repository.Sql

    Public Class SqlPerformerRepository
        Implements IPerformerRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Performer)) Implements IPerformerRepository.GetAsync
            Return Await _db.Performers.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Performer)) Implements IPerformerRepository.GetAsync

            Return Await _db.Performers.Where(Function(x As Performer) x.Name.Contains(search)).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Performer) Implements IPerformerRepository.GetAsyncExact
            Return Await _db.Performers.AsNoTracking().FirstOrDefaultAsync(Function(x As Performer) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Performer) Implements IPerformerRepository.GetAsync
            Return Await _db.Performers.AsNoTracking().FirstOrDefaultAsync(Function(x As Performer) x.Id = id)
        End Function

        Public Async Function Insert(Performer As Performer) As Task(Of Boolean) Implements IPerformerRepository.Insert
            If Await GetAsyncExact(Performer.Name) Is Nothing Then
                If Performer.Name.Contains("Carriger, Gail") Then
                    Dim x = 0
                End If
                Await _db.Performers.AddAsync(Performer)
                Await _db.SaveChangesAsync()
                Return True
            Else
                Return False
            End If
        End Function

        Public Async Function SetPerformers(Performers As List(Of Performer)) As Task
            Await ClearAsync()
            _db.Performers.AddRange(Performers)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddPerformers(Performers As List(Of Performer)) As Task
            _db.StartMassUpdate()
            For Each i In Performers
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements IPerformerRepository.ClearAsync
            For Each b In _db.Performers
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

    End Class

End Namespace
