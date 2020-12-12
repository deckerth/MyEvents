Imports MyEvents.App.ViewModels
Imports MyEvents.App.Views

Namespace Global.MyEvents.App.UserControls

    Public Class PerformanceYearChoiceFilter
        Inherits TextChoiceFilter

        Public Sub New()
            PopulateChoices()
        End Sub

        Private Sub PopulateChoices()
            Dim list As New List(Of ChoiceCheckBox)
            Dim years As New List(Of Integer)
            For Each e In EventListPage.Current.ViewModel.Events
                Dim perfDate As DateTime
                If DateTime.TryParse(e.PerformanceDate, perfDate) Then
                    If Not years.Contains(perfDate.Year) Then
                        list.Add(New ChoiceCheckBox With {.Value = perfDate.Year.ToString})
                        years.Add(perfDate.Year)
                    End If
                End If
            Next
            list.Add(New ChoiceCheckBox With {.Value = ""})
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, EventViewModel)
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso
                                            If(String.IsNullOrEmpty(x.Value),
                                               String.IsNullOrEmpty(entry.Venue),
                                               entry.PerformanceDate.Contains(x.Value, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function

    End Class

End Namespace
