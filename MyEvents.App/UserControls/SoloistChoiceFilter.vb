Imports MyEvents.App.ViewModels

Namespace Global.MyEvents.App.UserControls
    Public Class SoloistChoiceFilter
        Inherits TextChoiceFilter

        Public Sub New()
            PopulateChoices()
        End Sub

        Private Async Sub PopulateChoices()
            Dim performers = Await App.Repository.Soloists.GetAsync()
            Dim list As New List(Of ChoiceCheckBox)
            For Each e In performers
                list.Add(New ChoiceCheckBox With {.Value = e.Name})
            Next
            list.Add(New ChoiceCheckBox With {.Value = ""})
            SetChoicesFromList(list)
        End Sub

        Public Overrides Function PassesFilter(item As Object) As Boolean
            Dim entry = DirectCast(item, EventViewModel)
            Dim matches = Choices.Where(Function(x) x.IsChecked AndAlso
                                            If(String.IsNullOrEmpty(x.Value),
                                               String.IsNullOrEmpty(entry.Performer),
                                               entry.Performer.Contains(x.Value, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault()
            Return (matches IsNot Nothing)
        End Function

    End Class

End Namespace
