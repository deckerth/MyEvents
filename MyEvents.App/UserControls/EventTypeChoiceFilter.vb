Imports MyEvents.App.ValueConverters
Imports MyEvents.App.ViewModels
Imports MyEvents.Models

Namespace Global.MyEvents.App.UserControls
    Public Class EventTypeChoiceFilter
        Inherits TextChoiceFilter

        Public Sub New()
            If allEventTypes Is Nothing Then
                allEventTypes = New List(Of EventTypeDescriptor)
                For Each b In Performance.AllPerformanceTypes
                    allEventTypes.Add(New EventTypeDescriptor(b))
                Next
            End If
            PopulateChoices()
        End Sub

        Private Shared allEventTypes As List(Of EventTypeDescriptor)

        Private Sub PopulateChoices()
            Dim list As New List(Of ChoiceCheckBox)
            For Each e In allEventTypes
                list.Add(New ChoiceCheckBox With {.Value = e.Name})
            Next
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, EventViewModel)
            Dim eventType = New PerformanceTypeToStringConverter()
            Dim typeName = eventType.Convert(entry.Type, GetType(String), Nothing, "")
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso typeName.Equals(x.Value)).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function

    End Class

End Namespace
