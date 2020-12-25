Imports MyEvents.App.Statistics

Namespace Global.MyEvents.App.Views

    Public NotInheritable Class StatisticsPage
        Inherits Page

        Public Property ViewModel As New EventStatistics

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
            Await ViewModel.Initialize()
        End Sub

    End Class

End Namespace
