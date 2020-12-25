Imports MyEvents.Models
Imports MyEvents.Repository

Namespace Global.MyEvents.App.ViewModels
    Public Class MultipleEventsViewModel
        Inherits EventViewModel
        Implements INotifyPropertyChanged

        ' <summary>
        ' The underlying customer model. Friend so it Is 
        ' Not visible to the RadDataGrid. 
        ' </summary>
        Friend Property Models As List(Of EventViewModel)

        Private _works As New List(Of String)
        Public ReadOnly Property Works As List(Of String)
            Get
                Return _works
            End Get
        End Property
        Private WorksHaveBlanks As Boolean

        Private _composers As New List(Of String)
        Public ReadOnly Property Composers As List(Of String)
            Get
                Return _composers
            End Get
        End Property
        Private ComposersHaveBlanks As Boolean

        Private _soloists As New List(Of String)
        Public ReadOnly Property Soloists As List(Of String)
            Get
                Return _soloists
            End Get
        End Property
        Private SoloistsHaveBlanks As Boolean

        Private _directors As New List(Of String)
        Public ReadOnly Property Directors As List(Of String)
            Get
                Return _directors
            End Get
        End Property
        Private DirectorsHaveBlanks As Boolean

        Private _venues As New List(Of String)
        Public ReadOnly Property Venues As List(Of String)
            Get
                Return _venues
            End Get
        End Property
        Private VenuesHaveBlanks As Boolean

        Private _contributorsList As New List(Of String)
        Public ReadOnly Property ContributorsList As List(Of String)
            Get
                Return _contributorsList
            End Get
        End Property
        Private ContributorsHaveBlanks As Boolean

        Private _performanceDates As New List(Of String)
        Public ReadOnly Property PerformanceDates As List(Of String)
            Get
                Return _performanceDates
            End Get
        End Property
        Private PerformanceDatesHaveBlanks As Boolean

        Private _links As New List(Of String)
        Public ReadOnly Property Links As List(Of String)
            Get
                Return _links
            End Get
        End Property
        Private LinksHaveBlanks As Boolean

        Private _countries As New List(Of String)
        Public ReadOnly Property Countries As List(Of String)
            Get
                Return _countries
            End Get
        End Property
        Private CountriesHaveBlanks As Boolean

        Public Sub New(models As List(Of EventViewModel))
            MyBase.New(New Performance)
            If models Is Nothing Then
                Me.Models = New List(Of EventViewModel)
            Else
                Me.Models = models
            End If

            Work = App.Texts.GetString("KeepValue")
            Composer = App.Texts.GetString("KeepValue")
            Performer = App.Texts.GetString("KeepValue")
            Director = App.Texts.GetString("KeepValue")
            Venue = App.Texts.GetString("KeepValue")
            Contributors = App.Texts.GetString("KeepValue")
            PerformanceDate = App.Texts.GetString("KeepValue")
            Link = App.Texts.GetString("KeepValue")
            PerformanceCountry = App.Texts.GetString("KeepValue")
            SetupLists()
            IsInEdit = True
            IsModified = False
        End Sub

        Private Sub AddStringToList(ByRef storage As List(Of String), text As String, ByRef Optional BlanksFlag As Boolean = False)
            If String.IsNullOrEmpty(text) Then
                BlanksFlag = True
            Else
                If storage.Where(Function(x) x.Equals(text)).FirstOrDefault() Is Nothing Then
                    storage.Add(text)
                End If
            End If
        End Sub

        Private Sub AddEventToLists(item As EventViewModel)
            AddStringToList(_works, item.Work, WorksHaveBlanks)
            AddStringToList(_composers, item.Composer, ComposersHaveBlanks)
            AddStringToList(_soloists, item.Performer, SoloistsHaveBlanks)
            AddStringToList(_directors, item.Director, DirectorsHaveBlanks)
            AddStringToList(_venues, item.Venue, VenuesHaveBlanks)
            AddStringToList(_contributorsList, item.Contributors, ContributorsHaveBlanks)
            AddStringToList(_performanceDates, item.PerformanceDate, PerformanceDatesHaveBlanks)
            AddStringToList(_links, item.Link, LinksHaveBlanks)
            AddStringToList(_countries, item.PerformanceCountry, CountriesHaveBlanks)
        End Sub

        Private Sub AddDeleteEntryToLists()
            Dim deleteEntry = App.Texts.GetString("DeleteEntry")
            AddStringToList(_works, deleteEntry)
            AddStringToList(_composers, deleteEntry)
            AddStringToList(_soloists, deleteEntry)
            AddStringToList(_directors, deleteEntry)
            AddStringToList(_venues, deleteEntry)
            AddStringToList(_contributorsList, deleteEntry)
            AddStringToList(_performanceDates, deleteEntry)
            AddStringToList(_links, deleteEntry)
            AddStringToList(_countries, deleteEntry)
        End Sub

        Private Sub InitializeModelField(BlanksFlag As Boolean, values As List(Of String), ByRef field As String)
            If Not BlanksFlag AndAlso values.Count = 3 Then
                field = values.Item(2)
            End If
        End Sub

        Private Sub InitializeModelFields()
            InitializeModelField(WorksHaveBlanks, Works, Work)
            InitializeModelField(ComposersHaveBlanks, Composers, Composer)
            InitializeModelField(SoloistsHaveBlanks, Soloists, Performer)
            InitializeModelField(DirectorsHaveBlanks, Directors, Director)
            InitializeModelField(VenuesHaveBlanks, Venues, Venue)
            InitializeModelField(ContributorsHaveBlanks, ContributorsList, Contributors)
            InitializeModelField(PerformanceDatesHaveBlanks, PerformanceDates, PerformanceDate)
            InitializeModelField(CountriesHaveBlanks, Countries, PerformanceCountry)
        End Sub

        Private Sub SetupLists()
            AddEventToLists(Me) ' keep value
            AddDeleteEntryToLists()
            Dim keepEventType = New EventTypeDescriptor(Performance.PerformanceType.KeepValue)
            _allEventTypes.Insert(0, keepEventType)
            EventDescriptor = keepEventType
            For Each b In Models
                AddEventToLists(b)
            Next
            InitializeModelFields()
        End Sub

        Private Function UpdateString(ByRef storage As String, value As String) As Boolean
            If value.Equals(App.Texts.GetString("KeepValue")) Then
                Return False
            ElseIf value.Equals(App.Texts.GetString("DeleteEntry")) Then
                If Not String.IsNullOrEmpty(storage) Then
                    storage = ""
                    Return True
                Else
                    Return False
                End If
            ElseIf storage.Equals(value) Then
                Return False
            Else
                storage = value
                Return True
            End If
        End Function

        Private Function UpdateEvent(item As EventViewModel) As Boolean
            Dim changed As Boolean
            If item.PerformanceCountry Is Nothing Then
                item.PerformanceCountry = ""
            End If
            If item.Link Is Nothing Then
                item.Link = ""
            End If
            changed = UpdateString(item.Work, Work)
            changed = changed Or UpdateString(item.Composer, Composer)
            changed = changed Or UpdateString(item.Performer, Performer)
            changed = changed Or UpdateString(item.Director, Director)
            changed = changed Or UpdateString(item.Venue, Venue)
            changed = changed Or UpdateString(item.PerformanceDate, PerformanceDate)
            changed = changed Or UpdateString(item.PerformanceCountry, PerformanceCountry)
            changed = changed Or UpdateString(item.Link, Link)
            changed = changed Or UpdateString(item.Contributors, Contributors)
            If Type <> Performance.PerformanceType.KeepValue AndAlso Type <> item.Type Then
                changed = True
                item.EventDescriptor = EventDescriptor ' also changes the event type field
            End If
            Return changed
        End Function

        Public Async Function SaveAll() As Task(Of UpdateCounters)
            Dim counters = New UpdateCounters
            For Each b In Models
                If UpdateEvent(b) Then
                    Dim updateResult = Await App.Repository.Events.Upsert(b.Model)
                    If updateResult <> IEventRepository.UpsertResult.skipped Then
                        Await b.UpdateIndex()
                    End If
                    counters.Increment(updateResult)
                    b.IsModified = False
                Else
                    counters.Increment(IEventRepository.UpsertResult.skipped)
                End If
            Next
            IsModified = False
            Return counters
        End Function

        Public Overrides Async Function Save() As Task(Of IEventRepository.UpsertResult)
            Await SaveAll()
            Return IEventRepository.UpsertResult.updated
        End Function

        Protected Overrides Function ValidateAsyncOverride(propertyName As String) As Task
            For Each m In Models
                m.ValidateAsync(propertyName)
            Next

            Return MyBase.ValidateAsyncOverride(propertyName)
        End Function

        Protected Overrides Sub AddError(propertyName As String, errorMessage As Object)
            ' Skip errors
        End Sub

    End Class

End Namespace
