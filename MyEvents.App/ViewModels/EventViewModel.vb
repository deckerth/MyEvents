Imports MyEvents.App.Commands
Imports MyEvents.App.UserControls
Imports MyEvents.Models
Imports MyEvents.Repository
Imports Telerik.Core

Namespace Global.MyEvents.App.ViewModels

    Public Class EventViewModel
        Inherits ValidateViewModelBase
        Implements INotifyPropertyChanged

        Friend Property Model As Performance

        Public Sub New(model As Performance)
            If model Is Nothing Then
                Me.Model = New Performance
            Else
                Me.Model = model
            End If
            OpenLinkCommand = New RelayCommand(AddressOf OnOpenLink)
            DeleteCommand = New RelayCommand(AddressOf DeleteAsync)

            EventDescriptor = AllEventTypes.Where(Function(x) x.Type = Me.Model.Type).FirstOrDefault()
            If EventDescriptor Is Nothing Then
                EventDescriptor = New EventTypeDescriptor(Performance.PerformanceType.Undefined)
            End If
            SetTexts()
        End Sub

        Protected _allEventTypes As List(Of EventTypeDescriptor)
        Public ReadOnly Property AllEventTypes As List(Of EventTypeDescriptor)
            Get
                If _allEventTypes Is Nothing Then
                    _allEventTypes = New List(Of EventTypeDescriptor)
                    For Each b In Performance.AllPerformanceTypes
                        _allEventTypes.Add(New EventTypeDescriptor(b))
                    Next
                End If
                Return _allEventTypes
            End Get
        End Property

        Private _eventDescriptor As EventTypeDescriptor
        Public Property EventDescriptor As EventTypeDescriptor
            Get
                Return _eventDescriptor
            End Get
            Set(value As EventTypeDescriptor)
                If value IsNot Nothing AndAlso (_eventDescriptor Is Nothing OrElse _eventDescriptor.Type <> value.Type) Then
                    _eventDescriptor = value
                    Type = value.Type
                    OnPropertyChanged("EventDescriptor")
                End If
            End Set
        End Property

        Public Shared Event Modified()
        Public Shared Event Saved()

        ' <summary>
        ' Gets Or sets whether the underlying model has been modified. 
        ' This Is used when sync'ing with the server to reduce load
        ' And only upload the models that changed.
        ' </summary>
        Private _is_Modified As Boolean
        Friend Property IsModified As Boolean
            Get
                Return _is_Modified
            End Get
            Set(value As Boolean)
                If value <> _is_Modified Then
                    _is_Modified = value
                    If _is_Modified Then
                        RaiseEvent Modified()
                    End If
                End If
            End Set
        End Property

        Private _notification As NotificationViewModel = New NotificationViewModel(Me)
        Public ReadOnly Property Notification As NotificationViewModel
            Get
                Return _notification
            End Get
        End Property

        Public Property OpenLinkCommand As RelayCommand

        Private Async Sub OnOpenLink()
            If Not String.IsNullOrEmpty(Model.Link) Then
                Try
                    Dim uri = New Uri(Model.Link)
                    Await Windows.System.Launcher.LaunchUriAsync(uri)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Public Async Function Refresh() As Task
            Model = Await App.Repository.Events.GetAsync(Id)
            IsModified = False
        End Function

        Public Async Function UpdateIndex() As Task
            If Not String.IsNullOrWhiteSpace(Model.Contributors) Then
                Dim contributors = Model.GetContributorsList()
                For Each k In contributors
                    If Await App.Repository.Performers.Insert(New Models.Performer(k)) AndAlso App.AllPerformers.IsInitialized Then
                        App.AllPerformers.Contributors.Add(New Token With {.Text = k})
                    End If
                Next
            End If

            If Not String.IsNullOrWhiteSpace(Model.Composer) Then
                Await App.Repository.Composers.Insert(New Models.Composer(Model.Composer))
            End If

            If Not String.IsNullOrWhiteSpace(Model.Director) Then
                Await App.Repository.Directors.Insert(New Models.Director(Model.Director))
            End If

            If Not String.IsNullOrWhiteSpace(Model.Performer) Then
                Dim soloists = Model.GetSoloistsList()
                For Each s In soloists
                    If Await App.Repository.Soloists.Insert(New Models.Soloist(s)) AndAlso App.AllPerformers.IsInitialized Then
                        App.AllPerformers.Soloists.Add(New Token With {.Text = s})
                    End If
                Next
            End If

            If Not String.IsNullOrWhiteSpace(Model.Venue) Then
                Await App.Repository.Venues.Insert(New Models.Venue(Model.Venue, Model.PerformanceCountry))
            End If

            If Not String.IsNullOrWhiteSpace(Model.PerformanceCountry) Then
                Await App.Repository.Countries.Insert(New Models.Country(Model.PerformanceCountry))
            End If

        End Function

        Public Overridable Async Function Save() As Task(Of IEventRepository.UpsertResult)
            Await Notification.ScheduleNotificationAsync()
            Dim result = Await App.Repository.Events.Upsert(Model)
            IsModified = False
            RaiseEvent Saved()
            If result <> IEventRepository.UpsertResult.skipped Then
                Await UpdateIndex()
            End If
            Return result
        End Function

        Private Sub ScheduleNotification()

        End Sub

        Public Property IsInEdit As Boolean

        ' <summary>
        ' Gets Or sets whether to validate model data. 
        ' </summary>
        Friend Property Validate As Boolean

        Public Property Type As Performance.PerformanceType
            Get
                Return Model.Type
            End Get
            Set(value As Performance.PerformanceType)
                If value <> Model.Type Then
                    Model.Type = value
                    IsModified = True
                    OnPropertyChanged("Type")
                End If
            End Set
        End Property

        Public ReadOnly Property Id As Guid
            Get
                Return Model.Id
            End Get
        End Property

        Public Property Work As String
            Get
                Return Model.Work
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Work) Then
                    Model.Work = value
                    IsModified = True
                    OnPropertyChanged("Work")
                    SetTexts()
                End If
            End Set
        End Property
        Public Property Composer As String
            Get
                Return Model.Composer
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Composer) Then
                    Model.Composer = value
                    IsModified = True
                    OnPropertyChanged("Composer")
                    SetTexts()
                End If
            End Set
        End Property
        Public Property Performer As String
            Get
                Return Model.Performer
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Performer) Then
                    Model.Performer = value
                    IsModified = True
                    OnPropertyChanged("Performer")
                    SetTexts()
                End If
            End Set
        End Property
        Public Property Director As String
            Get
                Return Model.Director
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Director) Then
                    Model.Director = value
                    IsModified = True
                    OnPropertyChanged("Director")
                    SetTexts()
                End If
            End Set
        End Property
        Public Property Venue As String
            Get
                Return Model.Venue
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Venue) Then
                    Model.Venue = value
                    IsModified = True
                    OnPropertyChanged("Venue")
                End If
            End Set
        End Property
        Public Property Contributors As String
            Get
                Return Model.Contributors
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.Contributors) Then
                    Model.Contributors = value
                    IsModified = True
                    OnPropertyChanged("Contributors")
                    SetTexts()
                End If
            End Set
        End Property
        Public Property PerformanceDate As String
            Get
                Return Model.PerformanceDate
            End Get
            Set(value As String)
                If value IsNot Nothing AndAlso Not value.Equals(Model.PerformanceDate) Then
                    Model.PerformanceDate = value
                    IsModified = True
                    If Not String.IsNullOrWhiteSpace(Model.PerformanceDate) Then
                        Dim dummy As DateTime
                        If Not DateTime.TryParse(Model.PerformanceDate, dummy) Then
                            AddError("PerformanceDate", App.Texts.GetString("ErrorInvalidDate"))
                        Else
                            RemoveErrors("PerformanceDate")
                        End If
                    Else
                        RemoveErrors("PerformanceDate")
                    End If
                    OnPropertyChanged("PerformanceDate")
                End If
            End Set
        End Property

        Public Property Link As String
            Get
                Return Model.Link
            End Get
            Set(value As String)
                If Link Is Nothing OrElse Not Link.Equals(value) Then
                    Model.Link = value
                    IsModified = True
                    OnPropertyChanged("Link")
                End If
            End Set
        End Property

        Public Property PerformanceCountry As String
            Get
                Return Model.PerformanceCountry
            End Get
            Set(value As String)
                If Model.PerformanceCountry Is Nothing OrElse Not Model.PerformanceCountry.Equals(value) Then
                    Model.PerformanceCountry = value
                    IsModified = True
                    OnPropertyChanged("PerformanceCountry")
                End If
            End Set
        End Property

        Private _text1 As String = ""
        Public Property Text1 As String
            Get
                Return _text1
            End Get
            Set(value As String)
                If Not _text1.Equals(value) Then
                    _text1 = value
                    OnPropertyChanged("Text1")
                End If
            End Set
        End Property

        Private _text2 As String = ""
        Public Property Text2 As String
            Get
                Return _text2
            End Get
            Set(value As String)
                If Not _text2.Equals(value) Then
                    _text2 = value
                    OnPropertyChanged("Text2")
                End If
            End Set
        End Property

        Public Sub SetTexts()
            Dim texts As New List(Of String)
            If Not String.IsNullOrEmpty(Composer) Then
                texts.Add(Composer)
            End If
            If Not String.IsNullOrEmpty(Work) Then
                texts.Add(Work)
            End If
            If Not String.IsNullOrEmpty(Performer) Then
                texts.Add(Performer)
            End If
            If Not String.IsNullOrEmpty(Director) Then
                texts.Add(Director)
            End If
            If Not String.IsNullOrEmpty(Contributors) Then
                texts.Add(Contributors)
            End If
            Dim count As Integer = 0
            For Each s In texts
                If count = 0 Then
                    Text1 = s
                ElseIf count = 1 Then
                    Text2 = s
                Else
                    Exit For
                End If
                count += 1
            Next
            If count = 0 Then
                Text1 = ""
                Text2 = ""
            ElseIf count = 1 Then
                Text2 = ""
            End If
        End Sub

        Protected Overrides Function ValidateAsyncOverride(propertyName As String) As Task
            If String.IsNullOrEmpty(Work) Then
                AddError("Work", App.Texts.GetString("ErrorInvalidWork"))
            Else
                RemoveErrors("Work")
            End If

            Return MyBase.ValidateAsyncOverride(propertyName)
        End Function

        Public Property DeleteCommand As RelayCommand

        Public Shared Event Deleted(sender As EventViewModel)

        Public Async Function DeleteAsync() As Task
            Notification.RemoveNotification()
            Await App.Repository.Events.DeleteAsync(Id)
            RaiseEvent Deleted(Me)
        End Function

    End Class

End Namespace
