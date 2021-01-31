﻿Imports Windows.Foundation.Metadata
Imports Windows.System
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media.Animation

Namespace Global.MyEvents.App.Navigation

    Public Class NavMenuListView
        Inherits ListView


        Private splitViewHost As SplitView

        Public Sub New()
            SelectionMode = ListViewSelectionMode.Single

            ' This API doesn't exist on early versions of Windows 10, so check for it an only set it if it
            ' exists on the version that the app Is being run on.
            If ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Controls.ListViewBase", "SingleSelectionFollowsFocus") Then
                SingleSelectionFollowsFocus = False
            End If

            IsItemClickEnabled = True
            AddHandler ItemClick, AddressOf ItemClickedHandler
            AddHandler Loaded, AddressOf OnLoadedHandler
            AddHandler SelectionChanged, AddressOf SelectionChangedHandler

        End Sub

        Private Sub SelectionChangedHandler(sender As Object, e As SelectionChangedEventArgs)
            If lastSelectedItem IsNot Nothing AndAlso SelectedItem IsNot Nothing AndAlso lastSelectedIndex <> SelectedIndex Then
                SelectedItem = lastSelectedItem
            End If
        End Sub

        Private Sub OnPaneOpenPropertyChanged(sender As Object, e As Object)
            OnPaneToggled()
        End Sub

        Private Sub OnDisplayModePropertyChanged(sender As Object, e As Object)
            OnPaneToggled()
        End Sub

        Private Sub OnLoadedHandler(sender As Object, e As RoutedEventArgs)

            Dim parent = VisualTreeHelper.GetParent(Me)
            While (parent IsNot Nothing And TypeOf (parent) IsNot SplitView)
                parent = VisualTreeHelper.GetParent(parent)
            End While

            If parent IsNot Nothing Then
                splitViewHost = DirectCast(parent, SplitView)
                splitViewHost.RegisterPropertyChangedCallback(SplitView.IsPaneOpenProperty, AddressOf OnPaneOpenPropertyChanged)
                splitViewHost.RegisterPropertyChangedCallback(SplitView.DisplayModeProperty, AddressOf OnDisplayModePropertyChanged)
                OnPaneToggled()
            End If
        End Sub

        Protected Overrides Sub OnApplyTemplate()
            MyBase.OnApplyTemplate()
            For i = 0 To ItemContainerTransitions.Count - 1
                If TypeOf (ItemContainerTransitions(i)) Is EntranceThemeTransition Then
                    ItemContainerTransitions.RemoveAt(i)
                End If
            Next
        End Sub

        Private lastSelectedItem As ListViewItem = Nothing
        Private lastSelectedIndex As Integer = -1

        ' Mark the <paramref name="item"/> as selected And ensures everything else Is Not.
        ' If the <paramref name="item"/> Is null then everything Is unselected.
        Public Sub SetSelectedItem(item As ListViewItem)

            If item IsNot Nothing Then
                lastSelectedIndex = IndexFromContainer(item)
            End If

            For i = 0 To Items.Count - 1
                Dim lvi = DirectCast(ContainerFromIndex(i), ListViewItem)
                If i <> lastSelectedIndex And lvi IsNot Nothing Then
                    lvi.IsSelected = False
                ElseIf i = lastSelectedIndex Then
                    lvi.IsSelected = True
                    lastSelectedItem = lvi
                    SelectedItem = lvi
                End If
            Next

        End Sub

        Public Class ItemInvokedEventArgs
            Public Property NavigationAllowed As Boolean = True
            Public Property Item As ListViewItem
        End Class
        Public Event ItemInvoked(sender As Object, args As ItemInvokedEventArgs)

        Protected Overrides Sub OnKeyDown(e As KeyRoutedEventArgs)
            MyBase.OnKeyDown(e)
            Dim focusedItem = FocusManager.GetFocusedElement()
            Select Case e.Key
                Case VirtualKey.Up
                    TryMoveFocus(FocusNavigationDirection.Up)
                    e.Handled = True
                Case VirtualKey.Down
                    TryMoveFocus(FocusNavigationDirection.Down)
                    e.Handled = True
                Case VirtualKey.Space, VirtualKey.Enter
                    InvokeItem(focusedItem)
                    e.Handled = True
                Case Else
                    MyBase.OnKeyDown(e)
            End Select
        End Sub

        ' This method Is a work-around until the bug in FocusManager.TryMoveFocus Is fixed.
        Private Sub TryMoveFocus(direction As FocusNavigationDirection)
            If direction = FocusNavigationDirection.Next Or direction = FocusNavigationDirection.Previous Then
                FocusManager.TryMoveFocus(direction)
            Else
                Dim control As Control = FocusManager.FindNextFocusableElement(direction)
                If control IsNot Nothing Then
                    control.Focus(FocusState.Programmatic)
                End If
            End If
        End Sub

        Private Sub ItemClickedHandler(sender As Object, e As ItemClickEventArgs)
            ' Triggered when the item Is selected using something other than a keyboard
            Dim item = ContainerFromItem(e.ClickedItem)
            InvokeItem(item)
        End Sub

        Private Sub InvokeItem(focusedItem As Object)
            Dim args = New ItemInvokedEventArgs With {.Item = DirectCast(focusedItem, ListViewItem)}
            RaiseEvent ItemInvoked(Me, args)
            If args.NavigationAllowed Then
                SetSelectedItem(focusedItem)
                If splitViewHost.IsPaneOpen And (splitViewHost.DisplayMode = SplitViewDisplayMode.CompactOverlay Or splitViewHost.DisplayMode = SplitViewDisplayMode.Overlay) Then
                    splitViewHost.IsPaneOpen = False
                End If

                If TypeOf (focusedItem) Is ListViewItem Then
                    DirectCast(focusedItem, ListViewItem).Focus(FocusState.Programmatic)
                End If
            Else
                SetSelectedItem(lastSelectedItem)
            End If
        End Sub

        Private Sub OnPaneToggled()
            If splitViewHost.IsPaneOpen Then
                ItemsPanelRoot.ClearValue(FrameworkElement.WidthProperty)
                ItemsPanelRoot.ClearValue(FrameworkElement.HorizontalAlignmentProperty)
            ElseIf splitViewHost.DisplayMode = SplitViewDisplayMode.CompactInline Or splitViewHost.DisplayMode = SplitViewDisplayMode.CompactOverlay Then
                ItemsPanelRoot.SetValue(FrameworkElement.WidthProperty, splitViewHost.CompactPaneLength)
                ItemsPanelRoot.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left)
            End If
        End Sub

    End Class

End Namespace
