Imports Microsoft.QueryStringDotNET
Imports Microsoft.Toolkit.Uwp.Notifications
Imports Windows.UI.Notifications

Namespace Global.MyEvents.App.ViewModels

    Public Class NotificationViewModel

        Private _model As EventViewModel

        Public Class ToastContent
            Public Property Title As String
            Public Property PerformanceDate As String
        End Class

        Public Sub New(eventVM As EventViewModel)
            _model = eventVM
        End Sub

        Public Shared Function LoadExistingToasts() As Dictionary(Of String, Boolean)
            Dim result = New Dictionary(Of String, Boolean)
            Dim history As IReadOnlyList(Of ToastNotification) = ToastNotificationManager.History.GetHistory()

            For Each entry In history 'content als xml interpretieren
                Try
                    Dim toastNode = entry.Content.LastChild()
                    Dim launchString = toastNode.Attributes.GetNamedItem("launch").NodeValue.ToString
                    result.Add(launchString, False)
                Catch ex As Exception
                End Try
            Next
            Return result
        End Function

        Public Function GetQueryString() As QueryString
            Return New QueryString From {
                        {"title", _model.Text1},
                        {"date", _model.PerformanceDate}
                    }
        End Function

        Private Sub CreateNotification()
            Dim dateConverter = New ValueConverters.StringToDateTimeConverter()
            Dim tag = GetQueryString().ToString()

            Dim content = New ToastContentBuilder().AddText(_model.Text1).
                            AddText(_model.Text2).
                            AddText(_model.PerformanceDate).
                            AddToastActivationInfo(tag, ToastActivationType.Foreground).
                            GetToastContent()

            Dim eventDate As DateTime = dateConverter.Convert(_model.PerformanceDate, GetType(DateTime), Nothing, Nothing)
            Dim dueDate = eventDate.AddDays(-7)
            Dim expireDate = eventDate.AddDays(1)

            If Date.Now.CompareTo(dueDate) = -1 Then
                Dim toast = New ScheduledToastNotification(content.GetXml(), dueDate)
                toast.ExpirationTime = expireDate
                toast.Tag = tag
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast)
            Else
                Dim notif = New ToastNotification(content.GetXml())
                notif.ExpirationTime = expireDate
                notif.Tag = tag
                ToastNotificationManager.CreateToastNotifier().Show(notif)
            End If

        End Sub

        Public Shared Sub ScheduleNotifications(events As IReadOnlyList(Of EventViewModel))
            Dim existingToasts = NotificationViewModel.LoadExistingToasts()

            For Each i In events
                If i.Type = Models.Performance.PerformanceType.Planned Then
                    Dim queryString = i.Notification.GetQueryString().ToString
                    If existingToasts.ContainsKey(queryString) Then
                        existingToasts(queryString) = True
                    Else
                        i.Notification.CreateNotification()
                    End If
                End If
            Next
            For Each t In existingToasts
                If Not t.Value Then
                    ToastNotificationManager.History.Remove(t.ToString)
                End If
            Next
        End Sub

        Public Sub RemoveNotification()
            If _model.Type = Models.Performance.PerformanceType.Planned Then
                ToastNotificationManager.History.Remove(GetQueryString().ToString)
            End If
        End Sub

        Public Async Function ScheduleNotificationAsync() As Task
            Dim savedEvent = Await App.Repository.Events.GetAsync(_model.Id)
            Dim reschedule As Boolean = False
            Dim removeExisting As Boolean = False
            Dim savedModel As EventViewModel = Nothing
            If savedEvent IsNot Nothing AndAlso savedEvent.Type = Models.Performance.PerformanceType.Planned Then
                savedModel = New EventViewModel(savedEvent)
                If _model.Type = Models.Performance.PerformanceType.Planned Then
                    If Not savedModel.Notification.GetQueryString().ToString.Equals(_model.Notification.GetQueryString().ToString()) Then
                        reschedule = True
                        removeExisting = True ' planning details have changed
                    End If
                Else
                    removeExisting = True ' event is no longer planned
                End If
            Else
                reschedule = _model.Type = Models.Performance.PerformanceType.Planned
            End If
            If removeExisting Then
                ToastNotificationManager.History.Remove(savedModel.Notification.GetQueryString().ToString)
            End If
            If reschedule Then
                _model.Notification.CreateNotification()
            End If
        End Function

    End Class

End Namespace
