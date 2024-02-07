Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider
Imports MyEvents.Models

Namespace Global.MyEvents.Repository.Sql

    Public Class SqlVenueRepository
        Implements IVenueRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Venue)) Implements IVenueRepository.GetAsync
            Return Await _db.Venues.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Venue)) Implements IVenueRepository.GetAsync

            Return Await _db.Venues.AsNoTracking().Where(Function(x As Venue) x.Name.Contains(search)).ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Venue) Implements IVenueRepository.GetAsyncExact
            Return Await _db.Venues.AsNoTracking().FirstOrDefaultAsync(Function(x As Venue) x.Name = search)
        End Function

        Public Async Function GetAsyncExactWithTracking(search As String) As Task(Of Venue) Implements IVenueRepository.GetAsyncExactWithTracking
            Return Await _db.Venues.FirstOrDefaultAsync(Function(x As Venue) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Venue) Implements IVenueRepository.GetAsync
            Return Await _db.Venues.AsNoTracking().FirstOrDefaultAsync(Function(x As Venue) x.Id = id)
        End Function

        Public Async Function Insert(Venue As Venue) As Task Implements IVenueRepository.Insert
            Dim existing As Venue = Await GetAsyncExactWithTracking(Venue.Name)
            If existing Is Nothing Then
                Await _db.Venues.AddAsync(Venue)
                Await _db.SaveChangesAsync()
            ElseIf Not String.IsNullOrEmpty(Venue.Country) AndAlso Not Venue.Country.Equals(existing.Country) Then
                existing.Country = Venue.Country
                Try
                    _db.Venues.Update(existing)
                    Await _db.SaveChangesAsync()
                Catch ex As Exception
                    Dim x = 0
                End Try
            End If
        End Function

        Public Async Function SetVenues(Venues As List(Of Venue)) As Task
            Await ClearAsync()
            _db.Venues.AddRange(Venues)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddVenues(Venues As List(Of Venue)) As Task
            _db.StartMassUpdate()
            For Each i In Venues
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements IVenueRepository.ClearAsync
            For Each b In _db.Venues
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsyncExact(search As String) As Task Implements IVenueRepository.DeleteAsyncExact
            Dim toDelete = Await _db.Venues.AsNoTracking().FirstOrDefaultAsync(Function(x As Venue) x.Name = search)
            If toDelete IsNot Nothing Then
                _db.Entry(toDelete).State = EntityState.Deleted
            End If
            Await _db.SaveChangesAsync()
        End Function
    End Class

End Namespace
