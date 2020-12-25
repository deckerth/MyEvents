Imports MyEvents.App.ViewModels
Imports MyEvents.Models
Imports Telerik.UI.Xaml.Controls.Input

Namespace Global.MyEvents.App.Views
    Public NotInheritable Class EventDetailPage
        Inherits Page

        Public Property ViewModel As EventDetailPageViewModel

        Public Sub New()
            InitializeComponent()
            DataContext = ViewModel
        End Sub

        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            If e.Parameter IsNot Nothing Then
                If TypeOf e.Parameter Is MultipleEventsViewModel Then
                    ViewModel = New EventDetailPageViewModel(e.Parameter)
                Else
                    Dim performance As EventViewModel = DirectCast(e.Parameter, EventViewModel)
                    If ViewModel Is Nothing OrElse Not ViewModel.Performance.Equals(performance) Then
                        ViewModel = New EventDetailPageViewModel With {
                    .Performance = performance
                }
                        If performance.Id.Equals(Models.Performance.NewPerformanceId) Then
                            ViewModel.IsNewEvent = True
                            ViewModel.Performance.Model.Id = Guid.NewGuid()
                            PageHeaderText.Text = App.Texts.GetString("NewEvent")
                        End If
                        ViewModel.Performance.Validate = False
                        Bindings.Update()
                    End If
                End If
            Else
                ViewModel = New EventDetailPageViewModel With {
                    .IsNewEvent = True,
                    .Performance = New EventViewModel(New Models.Performance()) With {.Validate = True}
                }
                Bindings.Update()
                PageHeaderText.Text = App.Texts.GetString("NewEvent")
            End If

            ViewModel.IsInEdit = True

            MyBase.OnNavigatedTo(e)
        End Sub

        Private Async Function SaveChangesDialog() As Task(Of ContentDialogResult)
            Dim promtDialog As New ContentDialog With {
            .Title = "",
            .Content = App.Texts.GetString("DoYouWantToSave"),
            .CloseButtonText = App.Texts.GetString("No"),
            .PrimaryButtonText = App.Texts.GetString("Yes")
            }
            Return Await promtDialog.ShowAsync()
        End Function

        Private Sub CommandBar_Loaded(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub Save_Click(sender As Object, e As RoutedEventArgs)
            If Frame.CanGoBack Then
                Frame.GoBack()
            End If
        End Sub

        Private Async Sub CancelEditButton_Click(sender As Object, e As RoutedEventArgs)
            If ViewModel.Performance.IsModified Then
                Dim result As ContentDialogResult = Await SaveChangesDialog()
                If result = ContentDialogResult.Primary Then
                    ViewModel.SaveCommand.Execute(Nothing)
                Else
                    Await ViewModel.Performance.Refresh()
                End If
            End If

            If Frame.CanGoBack Then
                Frame.GoBack()
            End If

        End Sub

        Private Async Sub OnComposer_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim hits = Await App.Repository.Composers.GetAsync(sender.Text)
                Dim dataset As New List(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnDirector_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim hits As IEnumerable(Of Director) = Await App.Repository.Directors.GetAsync(sender.Text)
                Dim dataset As New List(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnSoloist_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
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
                Dim hits As IEnumerable(Of Soloist) = Await App.Repository.Soloists.GetAsync(input)
                Dim dataset As New List(Of String)
                For Each a In hits
                    dataset.Add(prefix + a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnVenue_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
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
                Dim dataset As New List(Of String)
                For Each a In hits
                    dataset.Add(prefix + a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnContributors_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim hits As IEnumerable(Of Performer) = Await App.Repository.Performers.GetAsync(sender.Text)
                Dim dataset As New List(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub OnCountry_TextChanged(sender As AutoSuggestBox, args As AutoSuggestBoxTextChangedEventArgs)
            If args.Reason = AutoSuggestionBoxTextChangeReason.UserInput Then
                Dim hits As IEnumerable(Of Country) = Await App.Repository.Countries.GetAsync(sender.Text)
                Dim dataset As New List(Of String)
                For Each a In hits
                    dataset.Add(a.Name)
                Next
                ' Set the ItemsSource to be your filtered dataset
                sender.ItemsSource = dataset
            End If
        End Sub

        Private Async Sub Venue_SuggestionChosen(sender As AutoSuggestBox, args As AutoSuggestBoxSuggestionChosenEventArgs) Handles Venue.SuggestionChosen
            Dim selected = args.SelectedItem
            Dim entry As Venue = Await App.Repository.Venues.GetAsyncExact(selected)
            If entry IsNot Nothing Then
                ViewModel.Performance.PerformanceCountry = entry.Country
            End If
        End Sub
    End Class

End Namespace
