Imports MyEvents.App.ValueConverters

Namespace Global.MyEvents.App.Statistics

    Public Class EventStatistics

        Public Property Events As IEnumerable(Of Models.Performance) = Nothing
        Public Property YearStatistics As New List(Of EventsPerYear)
        Public Property TypeStatistics As New List(Of EventsPerType)
        Public Property EventCounter As Integer
        Public Property MinYear As Double = Integer.MaxValue
        Public Property MaxYear As Double = Integer.MinValue

        Public Async Function Initialize() As Task
            Dim yearCounters As New Dictionary(Of Double, Double)
            Dim typeCounters As New Dictionary(Of Models.Performance.PerformanceType, Double)
            Dim eventsRegistry As New Dictionary(Of DateTime, Boolean)

            Events = Await App.Repository.Events.GetAsync()
            Dim dateConverter As New StringToDateTimeConverter
            Dim typeConverter As New PerformanceTypeToStringConverter

            For Each e In Events
                Dim dt As DateTime
                dt = dateConverter.Convert(e.PerformanceDate, GetType(DateTime), Nothing, Nothing)
                If dt.Year > 1900 Then
                    MinYear = Math.Min(MinYear, dt.Year)
                End If
                MaxYear = Math.Max(MaxYear, dt.Year)
                Dim count As Double = 0
                yearCounters.TryGetValue(dt.Year, count)
                yearCounters(dt.Year) = count + 1
                count = 0
                typeCounters.TryGetValue(e.Type, count)
                typeCounters(e.Type) = count + 1
                eventsRegistry.TryAdd(dt, True)
            Next

            For y = MinYear To MaxYear
                Dim count As Double = 0
                yearCounters.TryGetValue(y, count)
                YearStatistics.Add(New EventsPerYear With {.Year = y, .Count = count})
            Next

            For Each e In typeCounters
                If e.Key <> Models.Performance.PerformanceType.Undefined Then
                    Dim type As String = typeConverter.Convert(e.Key, GetType(String), Nothing, Nothing)
                    TypeStatistics.Add(New EventsPerType With {.Type = type, .Count = e.Value})
                End If
            Next

            EventCounter = eventsRegistry.Count
        End Function

    End Class


End Namespace
