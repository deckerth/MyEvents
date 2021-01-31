Namespace Global.MyEvents.App.ValueConverters

    Public Class DateComputations

        Private Shared converter As New StringToDateTimeConverter

        Public Shared Function DaysUntil(aDate As String) As String
            Dim result As String = ""
            If aDate IsNot Nothing Then
                Dim noDate As DateTime
                Dim today = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                Dim dt As DateTime = converter.Convert(aDate, GetType(DateTime), Nothing, Nothing)
                If dt <> noDate Then
                    Dim span = dt - today
                    Dim days = Math.Floor(span.TotalDays)
                    Dim negativeDays = -days
                    If days < 1 Then
                        result = App.Texts.GetString("DateInThePast").Replace("&", negativeDays.ToString)
                    ElseIf days = -1 Then
                        result = App.Texts.GetString("Yesterday")
                    ElseIf days = 0 Then
                        result = App.Texts.GetString("Today")
                    ElseIf days = 1 Then
                        result = App.Texts.GetString("Tomorrow")
                    Else
                        result = App.Texts.GetString("DateInTheFuture").Replace("&", days.ToString)
                    End If
                End If
            End If
            Return result
        End Function

    End Class

End Namespace
