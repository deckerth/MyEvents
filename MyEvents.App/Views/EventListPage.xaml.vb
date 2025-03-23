Imports System.Collections.Specialized
Imports Microsoft.QueryStringDotNET
Imports Microsoft.Toolkit.Uwp.Helpers
Imports MyEvents.App.ValueConverters
Imports MyEvents.App.ViewModels
Imports MyEvents.Models
Imports Telerik.Data.Core
Imports Telerik.UI.Xaml.Controls.Grid
Imports Telerik.UI.Xaml.Controls.Grid.Commands
Imports Telerik.UI.Xaml.Controls.Input
Imports Windows.System

Namespace Global.MyEvents.App.Views

    Public Class CustomCommitEditCommand
        Inherits DataGridCommand

        Public Sub New()
            Id = CommandId.CommitEdit
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            Dim context As EditContext = DirectCast(parameter, EditContext)
            EventListPage.Current.ViewModel.SelectedEvent.IsInEdit = False
            Owner.CommandService.ExecuteDefaultCommand(CommandId.CommitEdit, context)
            EventListPage.Current.ViewModel.SyncCommand.Execute(Nothing)
        End Sub
    End Class

    Public Class CustomCancelEditCommand
        Inherits DataGridCommand

        Public Sub New()
            Id = CommandId.CancelEdit
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Async Sub Execute(parameter As Object)
            Dim context As EditContext = DirectCast(parameter, EditContext)
            EventListPage.Current.ViewModel.SelectedEvent.IsInEdit = False
            Await EventListPage.Current.ViewModel.SelectedEvent.Refresh()
            Owner.CommandService.ExecuteDefaultCommand(CommandId.CancelEdit, context)
        End Sub
    End Class

    Public Class CustomBeginEditCommand
        Inherits DataGridCommand

        Public Sub New()
            Id = CommandId.BeginEdit
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            If EventListPage.Current.ViewModel.SelectedEvent IsNot Nothing Then
                Dim context As EditContext = DirectCast(parameter, EditContext)
                EventListPage.Current.ViewModel.SelectedEvent.IsInEdit = True
                Owner.CommandService.ExecuteDefaultCommand(CommandId.BeginEdit, context)
            End If
        End Sub
    End Class

    Public Class ComposerDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class WorkDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class DirectorDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class SoloistDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class VenueDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class PerformerDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class PerformanceDateDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class CountryDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class LinkDataTemplateSelector
        Inherits DataTemplateSelector

        Public Property EditTemplate As DataTemplate
        Public Property DisplayTemplate As DataTemplate

        Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
            Dim row = DirectCast(item, EventViewModel)
            If row.IsInEdit Then
                Return EditTemplate
            Else
                Return DisplayTemplate
            End If
        End Function
    End Class

    Public Class PerformanceDateIKeyLookup
        Implements IKeyLookup

        Public Function GetKey(instance As Object) As Object Implements IKeyLookup.GetKey
            Dim performance = DirectCast(instance, EventViewModel)
            Dim performanceDate As DateTime
            If performance.PerformanceDate IsNot Nothing AndAlso DateTime.TryParse(performance.PerformanceDate, performanceDate) Then
                Return performanceDate.Year
            End If
            Return Nothing
        End Function

    End Class

    Public NotInheritable Class EventListPage
        Inherits Page

        Public Shared Current As EventListPage
        Public Property ViewModel As EventListPageViewModel

        Private ComposerWorkSortDescriptors As New List(Of SortDescriptorBase)

        Private _dispatcherQueue As DispatcherQueue

        Public Sub New()
            InitializeComponent()
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread()
            Current = Me
            ComposerWorkSortDescriptors.Add(DataGrid.SortDescriptors.Item(0)) 'Composer
            ComposerWorkSortDescriptors.Add(DataGrid.SortDescriptors.Item(1)) 'Work
            NavigationCacheMode = NavigationCacheMode.Enabled
            AddHandler DataGrid.GroupDescriptors.CollectionChanged, AddressOf OnGroupCollectionChanged
        End Sub

        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
            ViewModel = e.Parameter
            ViewModel.SelectedItems = DataGrid.SelectedItems
            AddHandler ViewModel.SelectAll, AddressOf OnSelectAll
            AddHandler ViewModel.DeselectAll, AddressOf OnDeselectAll
            AddHandler ViewModel.ResetSorting, AddressOf OnResetSorting
            AddHandler ViewModel.SortComposerWork, AddressOf OnSortComposerWork
            DataContext = ViewModel
            AddHandler ViewModel.ActivationEventSelected, AddressOf OnActivationEventSelected
        End Sub

        Private Sub OnActivationEventSelected()
            ViewModel.LogText += "+OnActivationEventSelected"
            If ViewModel.SelectedEvent IsNot Nothing Then
                ViewModel.LogText += "+Register to scroll"
                EditEvent_Click(Me, New RoutedEventArgs())
            End If
        End Sub

        Private Sub OnGroupCollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs)
            Dim grouping As GroupDescriptorCollection = DirectCast(sender, GroupDescriptorCollection)
            For Each column In DataGrid.Columns
                Dim templateColumn As DataGridTemplateColumn = TryCast(column, DataGridTemplateColumn)
                If templateColumn IsNot Nothing Then
                    column.CanUserGroup = Not grouping.Contains(templateColumn.GroupDescriptor)
                End If
            Next
        End Sub

        Private Sub CreateEvent_Click(sender As Object, e As RoutedEventArgs)
            Frame.Navigate(GetType(EventDetailPage))
        End Sub

        Private Sub DuplicateEvent_Click(sender As Object, e As RoutedEventArgs)
            If DataGrid.SelectedItems.Count = 1 Then
                Dim duplicate = DataGrid.SelectedItems.ElementAt(0).Model.Clone()
                duplicate.Id = Performance.NewPerformanceId
                Dim duplicateViewModel = New EventViewModel(duplicate)
                Frame.Navigate(GetType(EventDetailPage), duplicateViewModel)
            End If
        End Sub

        Private Sub EditEvent_Click(sender As Object, e As RoutedEventArgs)
            If DataGrid.SelectedItems.Count > 0 Then
                If DataGrid.SelectedItems.Count = 1 Then
                    Frame.Navigate(GetType(EventDetailPage), DataGrid.SelectedItems.ElementAt(0))
                Else
                    Dim eventSet As New List(Of EventViewModel)
                    For Each b In ViewModel.SelectedItems
                        eventSet.Add(b)
                    Next
                    Dim eventSetViewModel As New MultipleEventsViewModel(eventSet)
                    Frame.Navigate(GetType(EventDetailPage), eventSetViewModel)
                End If
            End If
        End Sub

        Private Async Function ClearFilter() As Task
            If ViewModel.FilterIsSet Then
                ViewModel.FilterIsSet = False
                If ViewModel.IsBackupValid() Then
                    _dispatcherQueue.TryEnqueue(Sub() ViewModel.RestoreBackup())
                Else
                    _dispatcherQueue.TryEnqueue(Async Function()
                                                    Await ViewModel.GetEventsListAsync()
                                                End Function)
                    Await ViewModel.Progress.HideAsync()
                End If
            End If
        End Function

        Private Async Sub EventSearchBox_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            ' We only want to get results when it was a user typing,
            ' otherwise we assume the value got filled in by TextMemberPath
            ' Or the handler for SuggestionChosen.
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                If String.IsNullOrEmpty(sender.Text) Then
                    'Await DispatcherHelper.ExecuteOnUIThreadAsync(Async Function()
                    '                                                  Await ViewModel.GetBooksListAsync()
                    '                                              End Function)
                    sender.ItemsSource = Nothing
                    Await ClearFilter()
                Else
                    Dim parameters As String() = sender.Text.Split({" ", ",", ":", ";"}, StringSplitOptions.RemoveEmptyEntries)
                    sender.ItemsSource = ViewModel.Events.Where(
                        Function(x As EventViewModel) parameters.Any(Function(y As String) x.Work.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Composer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Performer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Contributors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Venue.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).OrderByDescending(
                        Function(x As EventViewModel) parameters.Count(Function(y As String) x.Work.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Composer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Performer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Contributors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Venue.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).Select(Of String)(
                        Function(x As EventViewModel) As String
                            Return x.Composer + ":" + x.Work
                        End Function)
                End If
            End If
        End Sub

        Private Async Sub EventSearchBox_QuerySubmitted(sender As AutoSuggestBox, args As AutoSuggestBoxQuerySubmittedEventArgs)
            If String.IsNullOrEmpty(args.QueryText) Then
                Await ClearFilter()
            Else
                Dim parameters As String() = args.QueryText.Split({" ", ",", ":", ";"}, StringSplitOptions.RemoveEmptyEntries)
                Dim matches = ViewModel.Events.Where(
                        Function(x As EventViewModel) parameters.Any(Function(y As String) x.Work.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Composer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Performer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Contributors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Venue.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).OrderByDescending(
                        Function(x As EventViewModel) parameters.Count(Function(y As String) x.Work.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Composer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Performer.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Contributors.Contains(y, StringComparison.OrdinalIgnoreCase) Or
                                                                         x.Venue.StartsWith(y, StringComparison.OrdinalIgnoreCase))
                        ).ToList()
                _dispatcherQueue.TryEnqueue(Sub()
                                                ViewModel.CreateBackup()
                                                ViewModel.Events.Clear()
                                                For Each match In matches
                                                    ViewModel.Events.Add(match)
                                                Next
                                                ViewModel.FilterIsSet = True
                                            End Sub)

            End If

        End Sub

        Private Sub CommandBar_Loaded(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub EventSearchBox_Loaded(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Async Sub OnComposer_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, e As AutoSuggestBoxTextChangedEventArgs)
            Dim hits As IEnumerable(Of Composer) = Await App.Repository.Composers.GetAsync(sender.Text)
            Dim dataset As New Collection(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset
        End Sub

        Private Async Sub OnComposer_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Composers.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Async Sub OnDirector_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim hits As IEnumerable(Of Director) = Await App.Repository.Directors.GetAsync(sender.Text)
                Dim dataset As New Collection(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnDirector_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Directors.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Async Sub OnSoloist_TextChanged(sender As RadAutoCompleteBox, e As TextChangedEventArgs)
            Dim hits As IEnumerable(Of Soloist) = Await App.Repository.Soloists.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset
        End Sub

        Private Async Sub OnVenue_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim input As String = sender.Text.Trim()
                Dim prefix As String = ""
                Dim lastSlash = input.LastIndexOf("/")
                If lastSlash > 0 Then
                    If input.Length = lastSlash + 1 Then
                        Return ' wait for futher input
                    Else
                        prefix = input.Substring(0, lastSlash + 1) + " "
                        input = input.Substring(lastSlash + 1).Trim()
                    End If
                End If
                Dim hits As IEnumerable(Of Venue) = Await App.Repository.Venues.GetAsync(input)
                Dim dataset As New Collection(Of String)
                For Each a In hits
                    dataset.Add(prefix + a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnVenue_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Venues.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Async Sub OnPerformer_TextChanged(sender As RadAutoCompleteBox, e As TextChangedEventArgs)
            Dim hits As IEnumerable(Of Performer) = Await App.Repository.Performers.GetAsync(sender.Text)
            Dim dataset As New List(Of String)
            For Each a In hits
                dataset.Add(a.Name)
            Next
            ' Set the ItemsSource to be your filtered dataset
            sender.ItemsSource = dataset
        End Sub

        Private Async Sub OnCountry_TextChanged(sender As UserControls.AdvancedAutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim hits As IEnumerable(Of Country) = Await App.Repository.Countries.GetAsync(sender.Text)
                Dim dataset As New Collection(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnCountry_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)
            Await App.Repository.Countries.DeleteAsyncExact(e.SuggestionToDelete)
        End Sub

        Private Sub OnSelectAll()
            DataGrid.SelectAll()
        End Sub

        Private Sub OnDeselectAll()
            DataGrid.DeselectAll()
        End Sub

        Private Sub DataGrid_SelectionChanged(sender As Object, e As DataGridSelectionChangedEventArgs)
            ViewModel.DataGrid_SelectionChanged()
        End Sub

#Region "Sorting"
        Private Sub OnResetSorting()
            DataGrid.SortDescriptors.Clear()
        End Sub

        Private Sub OnSortComposerWork()
            OnResetSorting()
            DataGrid.SortDescriptors.Add(ComposerWorkSortDescriptors.Item(0)) 'Composer
            DataGrid.SortDescriptors.Add(ComposerWorkSortDescriptors.Item(1)) 'Work
        End Sub

        Private Sub AdvancedAutoSuggestBox_DeleteSuggestion(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)

        End Sub

        Private Sub AdvancedAutoSuggestBox_DeleteSuggestion_1(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)

        End Sub

        Private Sub AdvancedAutoSuggestBox_DeleteSuggestion_2(sender As UserControls.AdvancedAutoSuggestBox, e As UserControls.AdvancedAutoSuggestBoxDeleteSuggestionArgs)

        End Sub

#End Region
    End Class

End Namespace
