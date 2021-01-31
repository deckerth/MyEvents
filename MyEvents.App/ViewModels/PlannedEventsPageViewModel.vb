Imports Microsoft.Toolkit.Uwp.Helpers
Imports Windows.System

Namespace Global.MyEvents.App.ViewModels

    Public Class PlannedEventsPageViewModel

        Public Property PlannedEvents As New ObservableCollection(Of EventViewModel)
        Private dispatcherQueue As DispatcherQueue

        Public Sub New()
            AddHandler EventDetailPageViewModel.OnNewEventCreated, AddressOf OnEventCreated
            AddHandler EventViewModel.Deleted, AddressOf OnEventDeleted
            dispatcherQueue = DispatcherQueue.GetForCurrentThread()
        End Sub

        Public Sub Initialize(allEvents As IEnumerable(Of EventViewModel))
            PlannedEvents.Clear()
            For Each i In allEvents
                If i.Type = Models.Performance.PerformanceType.Planned Then
                    PlannedEvents.Add(i)
                End If
            Next
        End Sub

        Private Sub OnEventCreated(newEvent As EventViewModel)
            If newEvent.Type = Models.Performance.PerformanceType.Planned Then
                dispatcherQueue.TryEnqueue(Sub() PlannedEvents.Add(newEvent))
            End If
        End Sub

        Private Sub OnEventDeleted(deletedEvent As EventViewModel)
            If deletedEvent.Type = Models.Performance.PerformanceType.Planned Then
                dispatcherQueue.TryEnqueue(Sub() PlannedEvents.Remove(deletedEvent))
            End If
        End Sub

    End Class

End Namespace
