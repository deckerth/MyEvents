Imports Windows.ApplicationModel.Core
Imports Windows.System
Imports Windows.UI.Core
Imports Windows.UI.Xaml.Automation
Imports MyEvents.App.Navigation
Imports MyEvents.App.Views
Imports MyEvents.App.ViewModels

Namespace Global.MyEvents.App
    Public NotInheritable Class AppShell
        Inherits Page

        Private _isPaddingAdded As Boolean

        Private Shared _AppShell As AppShell
        Public Shared Property Current As AppShell
            Get
                Return _AppShell
            End Get
            Private Set(value As AppShell)
                _AppShell = value
            End Set
        End Property

        Private _coreView As CoreApplicationView
        Public ReadOnly Property CoreView As CoreApplicationView
            Get
                Return _coreView
            End Get
        End Property

        Public Property AppViewModel As New AppShellViewModel

        Public Sub New()
            _AppShell = Me
            InitializeComponent()
            AddHandler Loaded, AddressOf OnLoadedHandler
            AddHandler EventDetailPageViewModel.EditMode, AddressOf OnEditModeChanged
        End Sub

        Private Sub OnEditModeChanged(isActive As Boolean)
            NavMenuList.IsEnabled = Not isActive
        End Sub

        Private Sub OnLoadedHandler(sender As Object, e As RoutedEventArgs)
            Current = Me
            CheckTogglePaneButtonSizeChanged()
            _coreView = CoreApplication.GetCurrentView()
            Dim titleBar = CoreApplication.GetCurrentView().TitleBar
            AddHandler titleBar.IsVisibleChanged, AddressOf TitleBar_IsVisibleChanged
            AddHandler SystemNavigationManager.GetForCurrentView().BackRequested, AddressOf SystemNavigationManager_BackRequested
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible

            'Make sure that the first menu items is selected
            Dim container = DirectCast(NavMenuList.ContainerFromItem(PrimaryMenuItems.ElementAt(0)), ListViewItem)
            If container IsNot Nothing Then
                NavMenuList.SetSelectedItem(DirectCast(container, ListViewItem))
            End If
        End Sub


        Public PrimaryMenuItems As IReadOnlyCollection(Of NavMenuItem) = New ReadOnlyCollection(Of NavMenuItem)(
    {
      New NavMenuItem With {.Symbol = Symbol.Library,
                            .Label = App.Texts.GetString("Events"),
                            .DestPage = GetType(EventListPage), .IsSelected = True},
      New NavMenuItem With {.Symbol = 60165, ' = 0xEB05
                            .Label = App.Texts.GetString("Statistics"),
                            .DestPage = GetType(StatisticsPage), .IsSelected = False},
      New NavMenuItem With {.Symbol = Symbol.Calendar,
                            .Label = App.Texts.GetString("Planned"),
                            .DestPage = GetType(PlannedEventsPage), .IsSelected = False}
    })

        Public Sub AppShell_KeyDown(sender As Object, e As KeyRoutedEventArgs)
            Dim direction As FocusNavigationDirection = FocusNavigationDirection.None
            Select Case e.Key
                Case VirtualKey.Left
                Case VirtualKey.GamepadDPadLeft
                Case VirtualKey.GamepadLeftThumbstickLeft
                Case VirtualKey.NavigationLeft
                    direction = FocusNavigationDirection.Left
                Case VirtualKey.Right
                Case VirtualKey.GamepadDPadRight
                Case VirtualKey.GamepadLeftThumbstickRight
                Case VirtualKey.NavigationRight
                    direction = FocusNavigationDirection.Right

                Case VirtualKey.Up
                Case VirtualKey.GamepadDPadUp
                Case VirtualKey.GamepadLeftThumbstickUp
                Case VirtualKey.NavigationUp
                    direction = FocusNavigationDirection.Up

                Case VirtualKey.Down
                Case VirtualKey.GamepadDPadDown
                Case VirtualKey.GamepadLeftThumbstickDown
                Case VirtualKey.NavigationDown
                    direction = FocusNavigationDirection.Down
            End Select

            If direction <> FocusNavigationDirection.None And
                FocusManager.FindNextFocusableElement(direction) Is GetType(Control) Then
                Dim control As Control = DirectCast(FocusManager.FindNextFocusableElement(direction), Control)
                control.Focus(FocusState.Keyboard)
                e.Handled = True
            End If
        End Sub

        Private Sub SystemNavigationManager_BackRequested(sender As Object, e As BackRequestedEventArgs)
            Dim handled As Boolean = e.Handled
            BackRequested(handled)
            e.Handled = handled
        End Sub

        Private Sub BackRequested(ByRef handled As Boolean)
            ' Get a hold of the current frame so that we can inspect the app back stack.
            If AppFrame() Is Nothing Then
                Return
            End If

            ' Check to see if this Is the top-most page on the app back stack.
            If AppFrame.CanGoBack And Not handled Then
                ' If Not, set the event to handled And go back to the previous page in the app.
                handled = True
                AppFrame.GoBack()
            End If
        End Sub

        ' Navigate to the Page for the selected
        Private Sub NavMenuList_ItemInvoked(sender As Object, args As NavMenuListView.ItemInvokedEventArgs)

            Dim item = DirectCast(DirectCast(sender, NavMenuListView).ItemFromContainer(args.Item), NavMenuItem)

            If item IsNot Nothing Then
                If item.DestPage IsNot Nothing AndAlso item.DestPage IsNot AppFrame.CurrentSourcePageType Then
                    If AppFrame.CurrentSourcePageType Is GetType(EventDetailPage) Then
                        If Not EventDetailPage.CanBeLeft() Then
                            args.NavigationAllowed = False
                            Return
                        End If
                    ElseIf AppFrame.CurrentSourcePageType Is GetType(EventListPage) Then
                        If EventListPage.Current.ViewModel.IsModified Then
                            args.NavigationAllowed = False
                            Return
                        End If
                    End If
                    For Each i In PrimaryMenuItems
                        i.IsSelected = False
                    Next
                    item.IsSelected = True
                    AppFrame.Navigate(item.DestPage, AppViewModel.MainViewModel)
                End If
            End If
        End Sub

        Public Function AppFrame() As Frame
            Return frame
        End Function

        Public Property TogglePaneButtonRect As Rect


        ' Invoked when window title bar visibility changes, such as after loading Or in tablet mode
        ' Ensures correct padding at window top, between title bar And app content
        Private Sub TitleBar_IsVisibleChanged(sender As CoreApplicationViewTitleBar, args As Object)
            If Not _isPaddingAdded And sender.IsVisible() Then
                'add extra padding between window title bar And app content
                Dim extraPadding As Double = DirectCast(App.Current.Resources("DesktopWindowTopPadding"), Double)
                _isPaddingAdded = True
                Dim margin As Thickness = NavMenuList.Margin
                NavMenuList.Margin = New Thickness(margin.Left, margin.Top + extraPadding, margin.Right, margin.Bottom)
                margin = AppFrame.Margin
                AppFrame.Margin = New Thickness(margin.Left, margin.Top + extraPadding, margin.Right, margin.Bottom)
                margin = TogglePaneButton.Margin
                TogglePaneButton.Margin = New Thickness(margin.Left, margin.Top + extraPadding, margin.Right, margin.Bottom)
            End If

        End Sub


        ' Ensures the nav menu reflects reality when navigation Is triggered outside of
        ' the nav menu buttons.
        Private Sub OnNavigatingToPage(sender As Object, e As NavigatingCancelEventArgs)
            If e.NavigationMode = NavigationMode.Back Then
                Dim item = PrimaryMenuItems.Where(Function(p) p.DestPage Is e.SourcePageType).SingleOrDefault()
                If item Is Nothing And AppFrame.BackStackDepth > 0 Then
                    ' In cases where a page drills into sub-pages then we'll highlight the most recent
                    ' navigation menu item that appears in the BackStack
                    For Each entry In AppFrame.BackStack.Reverse()
                        item = PrimaryMenuItems.Where(Function(p) p.DestPage Is entry.SourcePageType).SingleOrDefault()
                        If item IsNot Nothing Then

                        End If
                    Next
                End If
                For Each i In PrimaryMenuItems
                    i.IsSelected = False
                Next

                If item Is Nothing Then
                    Return
                End If

                item.IsSelected = True
                Dim container = DirectCast(NavMenuList.ContainerFromItem(item), ListViewItem)

                ' While updating the selection state of the item prevent it from taking keyboard focus.  If a
                ' user Is invoking the back button via the keyboard causing the selected nav menu item to change
                ' then focus will remain on the back button.
                If container IsNot Nothing Then
                    container.IsTabStop = False
                End If

                NavMenuList.SetSelectedItem(container)

                If container IsNot Nothing Then
                    container.IsTabStop = True
                End If
            End If
        End Sub

        Public Event TogglePaneButtonRectChanged(shell As AppShell, r As Rect)

        ' Public method to allow pages to open SplitView's pane.
        ' Used for custom app shortcuts Like navigating left from page's left-most item
        Public Sub OpenNavePane()
            TogglePaneButton.IsChecked = True
            NavPaneDivider.Visibility = Visibility.Visible
        End Sub

        ' Hides divider when nav pane is closed.
        Private Sub RootSplitView_PaneClosed(sender As SplitView, args As Object)
            NavPaneDivider.Visibility = Visibility.Collapsed
        End Sub

        ' Callback when the SplitView's Pane is toggled closed.  When the Pane is not visible
        ' then the floating hamburger may be occluding other content in the app unless it Is aware.
        Private Sub TogglePaneButton_Unchecked(sender As Object, e As RoutedEventArgs)
            CheckTogglePaneButtonSizeChanged()
        End Sub

        ' Callback when the SplitView's Pane is toggled opened.
        ' Restores divider's visibility and ensures that margins around the floating hamburger are correctly set.
        Private Sub TogglePaneButton_Checked(sender As Object, e As RoutedEventArgs)
            NavPaneDivider.Visibility = Visibility.Visible
            CheckTogglePaneButtonSizeChanged()
        End Sub

        ' Check for the conditions where the navigation pane does Not occupy the space under the floating
        ' hamburger button And trigger the event.
        Private Sub CheckTogglePaneButtonSizeChanged()
            TogglePaneButtonRect = New Rect()
            RaiseEvent TogglePaneButtonRectChanged(Me, TogglePaneButtonRect)
        End Sub

        ' Enable accessibility on each nav menu item by setting the AutomationProperties.Name on each container
        ' using the associated Label of each item.
        Private Sub NavMenuItemContainerContentChanging(sender As ListViewBase, args As ContainerContentChangingEventArgs)
            If Not args.InRecycleQueue And args.Item IsNot Nothing AndAlso args.Item Is GetType(NavMenuItem) Then
                args.ItemContainer.SetValue(AutomationProperties.NameProperty, DirectCast(args.Item, NavMenuItem).Label)
            Else
                args.ItemContainer.ClearValue(AutomationProperties.NameProperty)
            End If
        End Sub


        Private Async Sub SettingsButton_Click(sender As Object, e As RoutedEventArgs)
            Dim settings = New Views.SettingsDialog
            Await settings.ShowAsync()
        End Sub

        Private Sub HelpButton_Click(sender As Object, e As RoutedEventArgs)
            'HelpPage.CloneTo(ViewModel)
            'ViewModel.BrowserPaneOpen = True
        End Sub
    End Class

End Namespace
