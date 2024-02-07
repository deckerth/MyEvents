Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider
Imports MyEvents.Models


Namespace Global.MyEvents.Repository.Sql
    Public Class SqlComposerRepository
        Implements IComposerRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Composer)) Implements IComposerRepository.GetAsync
            Return Await _db.Composers.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Composer)) Implements IComposerRepository.GetAsync

            Return Await _db.Composers.AsNoTracking().Where(Function(x As Composer) x.Name.ToLowerInvariant.Contains(search.ToLowerInvariant)).ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Composer) Implements IComposerRepository.GetAsyncExact
            Return Await _db.Composers.AsNoTracking().FirstOrDefaultAsync(Function(x As Composer) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Composer) Implements IComposerRepository.GetAsync
            Return Await _db.Composers.AsNoTracking().FirstOrDefaultAsync(Function(x As Composer) x.Id = id)
        End Function

        Public Async Function Insert(Composer As Composer) As Task Implements IComposerRepository.Insert
            If Await GetAsyncExact(Composer.Name) Is Nothing Then
                Await _db.Composers.AddAsync(Composer)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetComposers(Composers As List(Of Composer)) As Task
            Await ClearAsync()
            _db.Composers.AddRange(Composers)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddComposers(Composers As List(Of Composer)) As Task
            _db.StartMassUpdate()
            For Each i In Composers
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements IComposerRepository.ClearAsync
            For Each b In _db.Composers
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsyncExact(search As String) As Task Implements IComposerRepository.DeleteAsyncExact
            Dim toDelete = Await _db.Composers.AsNoTracking().FirstOrDefaultAsync(Function(x As Composer) x.Name = search)
            If toDelete IsNot Nothing Then
                _db.Entry(toDelete).State = EntityState.Deleted
            End If
            Await _db.SaveChangesAsync()
        End Function

    End Class

End Namespace
