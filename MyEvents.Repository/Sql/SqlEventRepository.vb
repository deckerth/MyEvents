Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.ChangeTracking
Imports MyEvents.ContextProvider
Imports MyEvents.Models

Namespace Global.MyEvents.Repository.Sql

    Public Class SqlEventRepository
        Implements IEventRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Performance)) Implements IEventRepository.GetAsync
            Return Await _db.Events.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Performance)) Implements IEventRepository.GetAsync

            Dim parameters As String() = search.Split(" ")
            Return Await _db.Events.Where(
                Function(x As Performance) parameters.Any(Function(y As String) x.Composer.Contains(y) Or
                                                                         x.Contributors.Contains(y) Or
                                                                         x.Director.Contains(y) Or
                                                                         x.Performer.Contains(y) Or
                                                                         x.Venue.Contains(y) Or
                                                                         x.Work.Contains(y) Or
                                                                         x.PerformanceCountry.Contains(y))
                ).OrderByDescending(
                Function(x As Performance) parameters.Count(Function(y As String) x.Composer.Contains(y) Or
                                                                         x.Contributors.Contains(y) Or
                                                                         x.Director.Contains(y) Or
                                                                         x.Performer.Contains(y) Or
                                                                         x.Venue.Contains(y) Or
                                                                         x.Work.Contains(y) Or
                                                                         x.PerformanceCountry.Contains(y))
                ).AsNoTracking().ToListAsync()

        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Performance) Implements IEventRepository.GetAsync
            Return Await _db.Events.AsNoTracking().FirstOrDefaultAsync(Function(x As Performance) x.Id = id)
        End Function

        Public Async Function GetAsync(work As String, composer As String, performedAt As String) As Task(Of Performance) Implements IEventRepository.GetAsync
            Return Await _db.Events.AsNoTracking().FirstOrDefaultAsync(Function(x As Performance) x.Work = work And x.Composer = composer And x.PerformanceDate = performedAt)
        End Function

        Private Async Function GetAsyncWithTracking(id As Guid) As Task(Of Performance)
            Return Await _db.Events.FirstOrDefaultAsync(Function(x As Performance) x.Id = id)
        End Function

        Public Async Function GetAsyncWithTracking(work As String, composer As String, performedAt As String) As Task(Of Performance)
            Return Await _db.Events.FirstOrDefaultAsync(Function(x As Performance) x.Work = work And x.Composer = composer And x.PerformanceDate = performedAt)
        End Function

        Public Async Function Upsert(performance As Performance) As Task(Of IEventRepository.UpsertResult) Implements IEventRepository.Upsert
            Dim result As IEventRepository.UpsertResult = IEventRepository.UpsertResult.skipped

            DisplayTrackedEntities(_db.ChangeTracker)

            Dim existing = Await GetAsyncWithTracking(performance.Id)
            If existing IsNot Nothing Then
                Try
                    If existing.UpdateFrom(performance) Then
                        _db.Events.Update(existing)
                        result = IEventRepository.UpsertResult.updated
                    End If
                Catch ex As Exception

                End Try
            Else
                existing = Await GetAsyncWithTracking(performance.Work, performance.Composer, performance.PerformanceDate)
                If existing Is Nothing Then
                    Await _db.Events.AddAsync(performance)
                    result = IEventRepository.UpsertResult.added
                Else
                    If existing.UpdateFrom(performance) Then
                        _db.Events.Update(existing)
                        result = IEventRepository.UpsertResult.updated
                    End If
                End If
            End If
            Await _db.SaveChangesAsync()
            DisplayTrackedEntities(_db.ChangeTracker)
            Return result
        End Function

        Public Async Function DeleteAsync(id As Guid) As Task Implements IEventRepository.DeleteAsync
            Dim toDelete As Performance = Await GetAsyncWithTracking(id)
            If toDelete IsNot Nothing Then
                _db.Events.Remove(toDelete)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetEvents(performances As List(Of Performance)) As Task(Of UpdateCounters)
            For Each b In _db.Events
                _db.Entry(b).State = EntityState.Deleted
            Next
            Try
                Await _db.SaveChangesAsync()
                _db.Events.AddRange(performances)
                Await _db.SaveChangesAsync()
            Catch ex As Exception
                Return New UpdateCounters()
            End Try
            Return New UpdateCounters With {.Added = performances.Count}
        End Function

        Public Async Function AddEvents(performances As List(Of Performance)) As Task(Of UpdateCounters)
            Dim counters As New UpdateCounters
            _db.StartMassUpdate()
            For Each b In performances
                counters.Increment(Await Upsert(b))
            Next
            Await _db.EndMassUpdateModeAsync()
            Return counters
        End Function

        Private Shared LoggingActive As Boolean = False

        Private Sub DisplayTrackedEntities(changeTracker As ChangeTracker)
            If Not LoggingActive Then
                Return
            End If

            Debug.WriteLine("")
            Dim entries = changeTracker.Entries()
            For Each entry In entries
                Debug.WriteLine("Entity Name: {0}", entry.Entity.GetType().FullName)
                Debug.WriteLine("Status: {0}", entry.State)
            Next
            Debug.WriteLine("")
            Debug.WriteLine("---------------------------------------")
        End Sub

    End Class

End Namespace
