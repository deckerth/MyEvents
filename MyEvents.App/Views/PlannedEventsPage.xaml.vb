Imports MyEvents.App.ViewModels

Namespace Global.MyEvents.App.Views

    Public NotInheritable Class PlannedEventsPage
        Inherits Page

        Public Property ViewModel As New PlannedEventsPageViewModel

        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
            Dim allEvents As EventListPageViewModel = e.Parameter
            ViewModel.Initialize(allEvents.Events)
            DataContext = ViewModel
        End Sub

        Private Sub CreateEvent_Click(sender As Object, e As RoutedEventArgs)
            Frame.Navigate(GetType(EventDetailPage), True)
        End Sub

        Private Sub PlannedEventsGrid_ItemClick(sender As Object, e As ItemClickEventArgs)
            If e.ClickedItem IsNot Nothing Then
                Frame.Navigate(GetType(EventDetailPage), DirectCast(e.ClickedItem, EventViewModel))
            End If
        End Sub
    End Class

End Namespace
