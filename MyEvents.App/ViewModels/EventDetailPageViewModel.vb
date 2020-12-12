Imports MyEvents.App.Commands
Imports MyEvents.Repository

Namespace Global.MyEvents.App.ViewModels

    Public Class EventDetailPageViewModel
        Inherits BindableBase

        Public Sub New()
            InitializeCommands()
        End Sub

        Public Sub New(eventSet As MultipleEventsViewModel)
            InitializeCommands()
            Events = eventSet
        End Sub

        Private Sub InitializeCommands()
            SaveCommand = New RelayCommand(Async Sub()
                                               Await Save()
                                           End Sub)
            CancelEditsCommand = New RelayCommand(AddressOf OnCancelEdits)
            StartEditCommand = New RelayCommand(AddressOf OnStartEdit)
        End Sub

        Public Shared Event OnNewEventCreated(eNewBookCreatedArgs As EventViewModel)

        Private _isLoading As Boolean = False
        ' <summary>
        ' Gets Or sets whether to show the data loading progress wheel. 
        ' </summary>
        Public Property IsLoading As Boolean
            Get
                Return _isLoading
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isLoading, value)
            End Set
        End Property

        Private _isNewEvent As Boolean = False
        ' <summary>
        ' Indicates whether this Is a New book. 
        ' </summary>
        Public Property IsNewEvent As Boolean
            Get
                Return _isNewEvent
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isNewEvent, value)
            End Set
        End Property

        Private _multipleEvents As Boolean = False
        Public Property MultipleEvents As Boolean
            Get
                Return _multipleEvents
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_multipleEvents, value)
            End Set
        End Property

        Private _isInEdit As Boolean = False
        ' <summary>
        ' Gets or sets the current edit mode 
        ' </summary>
        Public Property IsInEdit As Boolean
            Get
                Return _isInEdit
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isInEdit, value)
            End Set
        End Property

        Private _performance As EventViewModel
        Public Property Performance As EventViewModel
            Get
                Return _performance
            End Get
            Set(value As EventViewModel)
                If SetProperty(Of EventViewModel)(_performance, value) Then
                    If String.IsNullOrWhiteSpace(Performance.Work) Then
                        IsInEdit = True
                    End If
                End If
                MultipleEvents = False
                AddHandler _performance.ErrorsChanged, AddressOf OnErrorsChanged
            End Set
        End Property

        Private _events As MultipleEventsViewModel
        Public Property Events As MultipleEventsViewModel
            Get
                Return _events
            End Get
            Set(value As MultipleEventsViewModel)
                SetProperty(Of MultipleEventsViewModel)(_events, value)
                If value Is Nothing Then
                    MultipleEvents = False
                Else
                    Performance = value
                    MultipleEvents = True
                    IsInEdit = True
                    IsNewEvent = False
                    For Each b In Events.Models
                        AddHandler b.ErrorsChanged, AddressOf OnErrorsChanged
                    Next
                End If
            End Set
        End Property

        Private _errorText As String = Nothing
        ' <summary>
        ' Gets Or sets the error text.
        ' </summary>
        Public Property ErrorText As String
            Get
                Return _errorText
            End Get
            Set(value As String)
                SetProperty(Of String)(_errorText, value)
            End Set
        End Property

        Private errorBuffer As String

        Private Sub AppendErrors(msgs As IEnumerable)
            Dim errorMsg As String
            If msgs IsNot Nothing Then
                For Each msg In msgs
                    errorMsg = TryCast(msg, String)
                    If errorMsg IsNot Nothing Then
                        If errorBuffer.Length > 0 Then
                            errorBuffer = errorBuffer + vbCrLf
                        End If
                        errorBuffer = errorBuffer + errorMsg
                    End If
                Next
            End If
        End Sub

        Protected Sub OnErrorsChanged(sender As Object, e As DataErrorsChangedEventArgs)
            If _performance.HasErrors Then
                errorBuffer = ""
                AppendErrors(_performance.GetErrors("Title"))
                AppendErrors(_performance.GetErrors("BorrowedDate"))
                AppendErrors(_performance.GetErrors("Published"))
                ErrorText = errorBuffer
            Else
                ErrorText = Nothing
            End If
        End Sub

        Private existenceCheckTaskRunning As Boolean
        Private existenceCheckRequired As Boolean = False
        Private eventExistsDetected As Boolean = False

        Private Async Function CheckExistenceAsync() As Task
            existenceCheckTaskRunning = True
            Do
                existenceCheckRequired = False
                Dim existingCopy = Await App.Repository.Events.GetAsync(work:=_performance.Work, composer:=_performance.Composer, performedAt:=_performance.PerformanceDate)
                If existingCopy Is Nothing Then
                    If eventExistsDetected Then
                        eventExistsDetected = False
                        ErrorText = ""
                    End If
                Else
                    eventExistsDetected = True
                    ErrorText = App.Texts.GetString("BookDoesAlreadyExist")
                End If
            Loop While existenceCheckRequired
            existenceCheckTaskRunning = False
        End Function


        Public Sub CheckExistence()
            If existenceCheckTaskRunning Then
                existenceCheckRequired = True
            Else
                Dim task = CheckExistenceAsync()
            End If
        End Sub

        Public Property SaveCommand As RelayCommand

        ' <summary>
        ' Saves book data that has been edited.
        ' </summary>
        Public Async Function Save() As Task(Of IEventRepository.UpsertResult)
            Dim result As IEventRepository.UpsertResult = IEventRepository.UpsertResult.skipped
            If Not _performance.HasErrors() Then
                If _performance.PerformanceDate Is Nothing Then
                    _performance.PerformanceDate = Date.MinValue
                End If
                result = Await _performance.Save()
                If result = Repository.IEventRepository.UpsertResult.added Then
                    RaiseEvent OnNewEventCreated(_performance)
                End If
                If _performance.PerformanceDate.Equals(Date.MinValue) Then
                    _performance.PerformanceDate = Nothing
                End If
            End If
            Return result
        End Function

        Public Property CancelEditsCommand As RelayCommand

        ' <summary>
        ' Cancels any in progress edits.
        ' </summary>
        Private Sub OnCancelEdits()
            IsInEdit = False
        End Sub

        Public Property StartEditCommand As RelayCommand

        ' <summary>
        ' Starts editing.
        ' </summary>
        Private Sub OnStartEdit()
            IsInEdit = True
        End Sub

    End Class

End Namespace
