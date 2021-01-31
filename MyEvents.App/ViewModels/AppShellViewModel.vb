Imports Microsoft.Toolkit.Uwp.Helpers
Imports Windows.System

Namespace Global.MyEvents.App.ViewModels

    Public Class AppShellViewModel
        Inherits BindableBase

        Public Property MainViewModel As EventListPageViewModel

        Private _navigationAllowed As Boolean
        Public Property NavigationAllowed As Boolean
            Get
                Return _navigationAllowed
            End Get
            Set(value As Boolean)
                SetProperty(Of Boolean)(_navigationAllowed, value)
            End Set
        End Property

        Public Async Function SetNavigationAllowedAsync(value As Boolean) As Task
            'AppShell.Current.CoreView.DispatcherQueue.TryEnqueue(Sub() NavigationAllowed = value)
            If AppShell.Current.CoreView IsNot Nothing Then
                Await DispatcherHelper.ExecuteOnUIThreadAsync(AppShell.Current.CoreView, Sub() NavigationAllowed = value)
            Else
                NavigationAllowed = value
            End If
        End Function

        Public Sub New()
            MainViewModel = New EventListPageViewModel()
        End Sub

    End Class

End Namespace
