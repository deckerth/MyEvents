Imports System.Xml
Imports Microsoft.QueryStringDotNET
Imports Microsoft.Toolkit.Uwp.Helpers
Imports Microsoft.Toolkit.Uwp.Notifications
Imports MyEvents.App.Commands
Imports MyEvents.App.Global.MyEvents.App.Views
Imports MyEvents.App.Views
Imports MyEvents.Repository
Imports Windows.Storage
Imports Windows.Storage.Pickers
Imports Windows.Storage.Provider
Imports Windows.System
Imports Windows.UI.Notifications
Imports Windows.UI.Popups

Namespace Global.MyEvents.App.ViewModels

    Public Class EventListPageViewModel
        Inherits BindableBase

        Private Shared _current As EventListPageViewModel
        Public Shared ReadOnly Property Current As EventListPageViewModel
            Get
                If _current Is Nothing Then
                    _current = New EventListPageViewModel()
                End If
                Return _current
            End Get
        End Property

        Private _dispatcherQueue As DispatcherQueue

        Public Sub New()
            _current = Me
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread()
            Task.Run(AddressOf GetEventsListAsync)
            SyncCommand = New RelayCommand(AddressOf OnSync)
            ImportDbCommand = New RelayCommand(AddressOf OnImportDB)
            ExportDbCommand = New RelayCommand(AddressOf OnExportDB)
            DeleteEventCommand = New RelayCommand(AddressOf OnDeleteEvent)
            InitalizeSelectionCommands()
            InitalizeSortCommands()
            AddHandler EventViewModel.Modified, AddressOf OnEventModified
            AddHandler EventViewModel.Deleted, AddressOf OnEventDeleted
            AddHandler EventViewModel.Saved, AddressOf OnEventSaved
            AddHandler EventDetailPageViewModel.OnNewEventCreated, AddressOf OnEventCreated
        End Sub

        Private Cancelled As Boolean

#Region "Properties"

        Private _logText As String = "Hello world"
        Public Property LogText As String
            Get
                Return _logText
            End Get
            Set(value As String)
                SetProperty(Of String)(_logText, value, "LogText")
            End Set
        End Property

        Private _isModified As Boolean
        Public Property IsModified As Boolean
            Get
                Return _isModified
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_isModified, value, "IsModified")
            End Set
        End Property

        Private Sub OnEventModified()
            IsModified = True
        End Sub

        Private Sub OnEventSaved()
            IsModified = Events.Where(Function(e) e.IsModified).FirstOrDefault() IsNot Nothing
        End Sub

        Private Sub OnEventCreated(newBook As EventViewModel)
            _dispatcherQueue.TryEnqueue(Sub() _events.Add(newBook))
            If ListBackup IsNot Nothing Then
                ListBackup.Add(newBook)
            End If
        End Sub

        Private _events As ObservableCollection(Of EventViewModel) = New ObservableCollection(Of EventViewModel)
        Public Property Events As ObservableCollection(Of EventViewModel)
            Get
                Return _events
            End Get
            Set(value As ObservableCollection(Of EventViewModel))
                SetProperty(Of ObservableCollection(Of EventViewModel))(_events, value)
            End Set
        End Property

        Private _selectedEvent As EventViewModel
        Public Property SelectedEvent As EventViewModel
            Get
                Return _selectedEvent
            End Get
            Set(value As EventViewModel)
                SetProperty(Of EventViewModel)(_selectedEvent, value, "SelectedEvent")
                OnPropertyChanged("SelectedEvent.Link")
            End Set
        End Property

        Private _multipleSelectionMode As Boolean = False

        Public Property ActivationArgs As Object

        Public Property MultipleSelectionMode As Boolean
            Get
                Return _multipleSelectionMode
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_multipleSelectionMode, value)
            End Set
        End Property

        Private _errorText As String = ""
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

        Public Property Progress As New ProgressRingViewModel

        Public Property FilterIsSet As Boolean = False
#End Region

#Region "FullListBackup"
        Private ListBackup As List(Of EventViewModel) = Nothing

        Public Sub CreateBackup()
            ListBackup = Events.ToList()
        End Sub

        Public Function IsBackupValid() As Boolean
            Return ListBackup IsNot Nothing
        End Function

        Public Sub RestoreBackup()
            If ListBackup IsNot Nothing Then
                Events.Clear()
                For Each b In ListBackup
                    Events.Add(b)
                Next
                ListBackup = Nothing
            End If
        End Sub
#End Region

#Region "Selection"

        Public EnableSingleSelectionModeCommand As RelayCommand
        Public EnableMultipleSelectionModeCommand As RelayCommand
        Public SelectAllCommand As RelayCommand
        Public DeselectAllCommand As RelayCommand

        Private Sub InitalizeSelectionCommands()
            SelectAllCommand = New RelayCommand(AddressOf OnSelectAll)
            DeselectAllCommand = New RelayCommand(AddressOf OnDeselectAll)
            EnableMultipleSelectionModeCommand = New RelayCommand(AddressOf OnEnableMultipleSelectionMode)
            EnableSingleSelectionModeCommand = New RelayCommand(AddressOf OnEnableSingleSelectionMode)
        End Sub

        Public Property SelectedItems As ObservableCollection(Of Object)

        Public Event SelectAll()
        Public Event DeselectAll()

        Private Sub OnSelectAll()
            RaiseEvent SelectAll()
        End Sub

        Private Sub OnDeselectAll()
            RaiseEvent DeselectAll()
        End Sub

        Private Sub OnEnableSingleSelectionMode()
            MultipleSelectionMode = False
        End Sub

        Private Sub OnEnableMultipleSelectionMode()
            MultipleSelectionMode = True
        End Sub
#End Region

#Region "Sorting"

        Public ResetSortingCommand As RelayCommand
        Public SortComposerWorkCommand As RelayCommand

        Public Event ResetSorting()
        Public Event SortComposerWork()

        Private Sub InitalizeSortCommands()
            ResetSortingCommand = New RelayCommand(AddressOf OnResetSorting)
            SortComposerWorkCommand = New RelayCommand(AddressOf OnSortComposerWork)
        End Sub

        Private Sub OnResetSorting()
            RaiseEvent ResetSorting()
        End Sub

        Private Sub OnSortComposerWork()
            RaiseEvent SortComposerWork()
        End Sub

#End Region

#Region "DataAccess"
        Public Event ActivationEventSelected()

        Public Async Function GetEventsListAsync() As Task
            Await Progress.SetIndeterministicAsync()
            Try
                Dim repo = Await App.Repository.Events.GetAsync()
                If repo Is Nothing Then
                    Return
                End If
                Await DispatcherHelper.ExecuteOnUIThreadAsync(
                Sub()
                    If ActivationArgs Is Nothing Then
                        LogText = "ActivationArgs Is Nothing"
                    Else
                        LogText = "ActivationArgs Is Not Nothing"
                    End If
                    Events.Clear()
                    For Each b In repo
                        Dim vm = New EventViewModel(b) With {.Validate = True}
                        AddHandler vm.ErrorsChanged, AddressOf OnErrorsChanged
                        Events.Add(vm)
                    Next
                    Dim activationEvent = GetEventForActivation()
                    If activationEvent IsNot Nothing Then
                        LogText = activationEvent.PerformanceDate
                        SelectedEvent = activationEvent
                        RaiseEvent ActivationEventSelected()
                        'DataGrid.ScrollItemIntoView(activationEvent)
                        '                                              End Sub)
                    Else
                        LogText += " + Not found"
                    End If
                End Sub)
                NotificationViewModel.ScheduleNotifications(Events)

                Await Progress.HideAsync()

            Catch ex As Exception

            End Try
        End Function

        Private Sub OnErrorsChanged(sender As Object, e As DataErrorsChangedEventArgs)
            Dim vm = DirectCast(sender, EventViewModel)
            Dim errors = vm.GetErrors(e.PropertyName)
            Dim enumerator = errors.GetEnumerator()
            If enumerator.MoveNext() Then
                ErrorText = enumerator.Current
            Else
                ErrorText = ""
            End If
        End Sub

        Public Function GetEvent(title As String, eventDate As String) As EventViewModel
            Return Events.FirstOrDefault(Function(x) x.Text1.Equals(title) AndAlso x.PerformanceDate.Equals(eventDate))
        End Function

        ' The app was invoked by clicking on an event notification -> Navigate to the event
        Friend Function GetEventForActivation() As EventViewModel
            If ActivationArgs IsNot Nothing Then
                Try
                    Dim query = DirectCast(ActivationArgs, String)
                    LogText = LogText + "+1"
                    Dim args = QueryString.Parse(query)
                    LogText = LogText + "+2"
                    Dim dateStr = args("date")
                    LogText = LogText + "+3"
                    Dim title = args("title")
                    LogText = LogText + "+4"
                    Dim selectedEvent = GetEvent(title, dateStr)
                    LogText = LogText + "+5"
                    ActivationArgs = Nothing
                    LogText = LogText + "+6"
                    Return selectedEvent
                Catch ex As Exception
                    LogText = ex.ToString
                    Return Nothing
                End Try
            Else
                Return Nothing
            End If
        End Function

        Public Property SyncCommand As RelayCommand

        Private Async Function Synchronize() As Task
            Await Progress.SetIndeterministicAsync()

            Dim modifiedViewModels = Events.Where(Function(x) x.IsModified)
            For Each m In modifiedViewModels
                Await m.Save()
            Next

            _dispatcherQueue.TryEnqueue(Sub() IsModified = False)
            Await Progress.HideAsync()
        End Function

        Private Sub OnSync()
            Task.Run(AddressOf Synchronize)
        End Sub

        Public DeleteEventCommand As RelayCommand

        Private Sub OnEventDeleted(sender As EventViewModel)
            Events.Remove(sender)
            If ListBackup IsNot Nothing Then
                ListBackup.Remove(sender)
            End If
        End Sub

        Private Async Function DeleteEventsAsync(toDelete As List(Of EventViewModel)) As Task
            If toDelete IsNot Nothing Then
                Dim dialog = New MessageDialog(App.Texts.GetString("DeleteEventsQuestion").Replace("&", toDelete.Count.ToString))

                ' Add commands and set their callbacks 
                dialog.Commands.Add(New UICommand(App.Texts.GetString("Yes")))
                dialog.Commands.Add(New UICommand(App.Texts.GetString("Cancel"), Sub(command) Cancelled = True))

                Cancelled = False
                Await dialog.ShowAsync()

                If Cancelled = False Then
                    For Each b In toDelete
                        Try
                            Await b.DeleteAsync()
                        Catch ex As Exception
                        End Try
                    Next
                End If
            End If
        End Function

        Public Async Sub OnDeleteEvent()
            Await Synchronize() ' New events may not yet have been saved. Unsaved events cannot be deleted.
            If SelectedItems.Count > 0 Then
                If SelectedItems.Count = 1 Then
                    Await DeleteEventsAsync(SelectedItems.ElementAt(0))
                Else
                    Dim eventSet As New List(Of EventViewModel)
                    For Each b In SelectedItems
                        eventSet.Add(b)
                    Next
                    Await DeleteEventsAsync(eventSet)
                End If
            End If
        End Sub

#End Region

#Region "ImportExport"
        Public ImportDbCommand As RelayCommand
        Public ExportDbCommand As RelayCommand

        Public Async Function OnExportDB() As Task

            Dim savepicker As New FileSavePicker With {
            .SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        }

            ' Dropdown of file types the user can save the file as
            savepicker.FileTypeChoices.Add("Excel Workbook", New List(Of String) From {".xlsx"})
            ' Default file name if the user does Not type one in Or select a file to replace
            savepicker.SuggestedFileName = "MyEvents"

            Dim File As StorageFile = Await savepicker.PickSaveFileAsync()
            If File IsNot Nothing Then
                Using stream = Await File.OpenStreamForWriteAsync()
                    stream.SetLength(0)
                    ' Prevent updates to the remote version of the file until we finish making changes And call CompleteUpdatesAsync.  Does not work with Windows 11.
                    Dim updatesDeferred As Boolean
                    Try
                        CachedFileManager.DeferUpdates(File)
                        updatesDeferred = True
                    Catch ex As Exception
                        updatesDeferred = False
                    End Try
                    Await App.Repository.ImportExportService.ExportAsync(stream)

                    If updatesDeferred Then
                        Dim status As FileUpdateStatus = Await CachedFileManager.CompleteUpdatesAsync(File)
                        If status = FileUpdateStatus.Complete Then
                            Await New MessageDialog(App.Texts.GetString("DatabaseSaved")).ShowAsync()
                        Else
                            Await New MessageDialog(App.Texts.GetString("DatabaseNotSaved")).ShowAsync()
                        End If
                    Else
                        Await New MessageDialog(App.Texts.GetString("DatabaseSaved")).ShowAsync()
                    End If
                End Using
            End If
        End Function

        Private ImportDecisionDialogResult As IImportExportService.ImportOptions

        Private Async Function ImportDialog() As Task(Of Boolean)

            Dim dialog = New MessageDialog(App.Texts.GetString("ImportDecision"))

            ' Add commands and set their callbacks 
            dialog.Commands.Add(New UICommand(App.Texts.GetString("AddImport"), Sub(command) ImportDecisionDialogResult = IImportExportService.ImportOptions.AddEvents))
            dialog.Commands.Add(New UICommand(App.Texts.GetString("ReplaceCollection"), Sub(command) ImportDecisionDialogResult = IImportExportService.ImportOptions.ReplaceEvents))
            dialog.Commands.Add(New UICommand(App.Texts.GetString("Cancel"), Sub(command) Cancelled = True))

            Cancelled = False
            Await dialog.ShowAsync()

            Return Not Cancelled

        End Function

        Public Async Function OnImportDB() As Task

            If Await ImportDialog() Then
                Dim openPicker As FileOpenPicker = New FileOpenPicker()
                openPicker.ViewMode = PickerViewMode.Thumbnail
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                openPicker.FileTypeFilter.Add(".xlsx")
                Dim File As StorageFile = Await openPicker.PickSingleFileAsync()
                Dim Counters As UpdateCounters
                If File IsNot Nothing Then
                    Await Progress.SetIndeterministicAsync()
                    Counters = Await App.Repository.ImportExportService.ImportAsync(Await File.OpenStreamForReadAsync(), ImportDecisionDialogResult)
                    Await GetEventsListAsync()
                    Await Progress.HideAsync()
                    Dim dialog = New ImportResultDialog(Counters)
                    Await dialog.ShowAsync()
                    ListBackup = Nothing
                End If
            End If

        End Function

#End Region

#Region "UpdateIndex"

        Public Async Function UpdateIndexAsync() As Task
            Await Progress.SetDeterministicAsync(Events.Count)
            'App.Repository.StartMassUpdate()
            Await App.Repository.Composers.ClearAsync()
            Await App.Repository.Directors.ClearAsync()
            Await App.Repository.Performers.ClearAsync()
            Await App.Repository.Soloists.ClearAsync()
            Await App.Repository.Venues.ClearAsync()
            Await App.Repository.Countries.ClearAsync()
            Dim c = 0
            For Each e In Events
                Await e.UpdateIndex()
                c += 1
                Progress.IncrementOnDispatcher(1)
                If c >= 101 Then
                    c = c
                End If
            Next
            'Await App.Repository.EndMassUpdateAsync()

            Await Progress.HideAsync()
        End Function

#End Region

    End Class

End Namespace
