Imports MyEvents.App.ValueConverters
Imports MyEvents.Models

Namespace Global.MyEvents.App.ViewModels
    Public Class EventTypeDescriptor

        Public Property Type As Performance.PerformanceType
        Public Property Name As String

        Public Sub New()
        End Sub

        Public Sub New(aType As Performance.PerformanceType)
            Dim eventType = New PerformanceTypeToStringConverter()

            Type = aType
            Name = eventType.Convert(Type, GetType(String), Nothing, "")
        End Sub

    End Class

End Namespace
