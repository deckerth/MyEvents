Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider
Imports MyEvents.Models

Namespace Global.MyEvents.Repository.Sql
    Public Class SqlSoloistRepository
        Implements ISoloistRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Soloist)) Implements ISoloistRepository.GetAsync
            Return Await _db.Soloists.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Soloist)) Implements ISoloistRepository.GetAsync

            Return Await _db.Soloists.Where(Function(x As Soloist) x.Name.Contains(search)).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Soloist) Implements ISoloistRepository.GetAsyncExact
            Return Await _db.Soloists.AsNoTracking().FirstOrDefaultAsync(Function(x As Soloist) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Soloist) Implements ISoloistRepository.GetAsync
            Return Await _db.Soloists.AsNoTracking().FirstOrDefaultAsync(Function(x As Soloist) x.Id = id)
        End Function

        Public Async Function Insert(Soloist As Soloist) As Task Implements ISoloistRepository.Insert
            If Await GetAsyncExact(Soloist.Name) Is Nothing Then
                If Soloist.Name.Contains("Carriger, Gail") Then
                    Dim x = 0
                End If
                Await _db.Soloists.AddAsync(Soloist)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetSoloists(Soloists As List(Of Soloist)) As Task
            Await ClearAsync()
            _db.Soloists.AddRange(Soloists)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddSoloists(Soloists As List(Of Soloist)) As Task
            _db.StartMassUpdate()
            For Each i In Soloists
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements ISoloistRepository.ClearAsync
            For Each b In _db.Soloists
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

    End Class

End Namespace
