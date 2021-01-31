Imports MyEvents.App.UserControls

Public Class ContributorsViewModel

    Public Property Contributors As New List(Of Token)
    Public Property Soloists As New List(Of Token)

    Public Property IsInitialized As Boolean = False

    Public Async Function InitializeAsync() As Task
        If Not IsInitialized Then
            IsInitialized = True
            Dim allPerformers = Await App.Repository.Performers.GetAsync()
            Contributors.Clear()
            For Each c In allPerformers
                Contributors.Add(New Token With {.Text = c.Name})
            Next
            Dim allSoloists = Await App.Repository.Soloists.GetAsync()
            Soloists.Clear()
            For Each c In allSoloists
                Soloists.Add(New Token With {.Text = c.Name})
            Next
        End If
    End Function

End Class
