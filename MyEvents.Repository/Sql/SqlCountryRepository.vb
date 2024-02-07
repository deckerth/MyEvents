Imports Microsoft.EntityFrameworkCore
Imports MyEvents.ContextProvider
Imports MyEvents.Models


Namespace Global.MyEvents.Repository.Sql
    Public Class SqlCountryRepository
        Implements ICountryRepository

        Private ReadOnly _db As MyEventsContext

        Public Sub New(db As MyEventsContext)
            _db = db
        End Sub

        Public Async Function GetAsync() As Task(Of IEnumerable(Of Country)) Implements ICountryRepository.GetAsync
            Return Await _db.Countries.AsNoTracking().ToListAsync()
        End Function

        Public Async Function GetAsync(search As String) As Task(Of IEnumerable(Of Country)) Implements ICountryRepository.GetAsync

            Return Await _db.Countries.AsNoTracking().Where(Function(x As Country) x.Name.Contains(search)).ToListAsync()

        End Function

        Public Async Function GetAsyncExact(search As String) As Task(Of Country) Implements ICountryRepository.GetAsyncExact
            Return Await _db.Countries.AsNoTracking().FirstOrDefaultAsync(Function(x As Country) x.Name = search)
        End Function

        Public Async Function GetAsync(id As Guid) As Task(Of Country) Implements ICountryRepository.GetAsync
            Return Await _db.Countries.AsNoTracking().FirstOrDefaultAsync(Function(x As Country) x.Id = id)
        End Function

        Public Async Function Insert(Country As Country) As Task Implements ICountryRepository.Insert
            If Await GetAsyncExact(Country.Name) Is Nothing Then
                If Country.Name.Contains("Carriger, Gail") Then
                    Dim x = 0
                End If
                Await _db.Countries.AddAsync(Country)
                Await _db.SaveChangesAsync()
            End If
        End Function

        Public Async Function SetCountries(Countries As List(Of Country)) As Task
            Await ClearAsync()
            _db.Countries.AddRange(Countries)
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function AddCountries(Countries As List(Of Country)) As Task
            _db.StartMassUpdate()
            For Each i In Countries
                Await Insert(i)
            Next
            Await _db.EndMassUpdateModeAsync()
        End Function

        Public Async Function ClearAsync() As Task Implements ICountryRepository.ClearAsync
            For Each b In _db.Countries
                _db.Entry(b).State = EntityState.Deleted
            Next
            Await _db.SaveChangesAsync()
        End Function

        Public Async Function DeleteAsyncExact(search As String) As Task Implements ICountryRepository.DeleteAsyncExact
            Dim toDelete = Await _db.Countries.AsNoTracking().FirstOrDefaultAsync(Function(x As Country) x.Name = search)
            If toDelete IsNot Nothing Then
                _db.Entry(toDelete).State = EntityState.Deleted
            End If
            Await _db.SaveChangesAsync()
        End Function
    End Class

End Namespace
