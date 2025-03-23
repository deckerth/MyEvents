Imports Telerik.Data.Core
Imports Telerik.UI.Xaml.Controls.Grid.Commands
Imports Telerik.UI.Xaml.Controls.Grid.Primitives

Namespace Global.MyEvents.App.UserControls

    Public Class CustomFilterButtonTapCommand
        Inherits DataGridCommand

        Public Sub New()
            Me.Id = CommandId.FilterButtonTap
        End Sub

        Public Overrides Function CanExecute(parameter As Object) As Boolean
            Return True
        End Function

        Public Overrides Sub Execute(parameter As Object)
            Dim context = DirectCast(parameter, FilterButtonTapContext)

            If ColumnMarker.GetRequiresCustomFiltering(context.Column) Then
                Dim firstFilter As FilterDescriptorBase = Nothing
                Dim secondFilter As FilterDescriptorBase = Nothing
                If context.AssociatedDescriptor IsNot Nothing Then
                    If TypeOf (context.AssociatedDescriptor) Is CompositeFilterDescriptor Then
                        firstFilter = DirectCast(context.AssociatedDescriptor, CompositeFilterDescriptor).Descriptors.Item(0)
                        secondFilter = DirectCast(context.AssociatedDescriptor, CompositeFilterDescriptor).Descriptors.Item(1)
                    ElseIf TypeOf (context.AssociatedDescriptor) Is DelegateFilterDescriptor Then
                        firstFilter = DirectCast(context.AssociatedDescriptor, DelegateFilterDescriptor)
                    Else
                        firstFilter = DirectCast(context.AssociatedDescriptor, TextFilterDescriptor)
                    End If
                End If

                Dim columnName = ColumnMarker.GetColumnName(context.Column)
                If ColumnMarker.GetIsTextFilter(context.Column) Then
                    Dim textFilter = New DataGridTextFilterControl With {
                        .DataContext = firstFilter,
                        .PropertyName = columnName
                    }
                    context.FirstFilterControl = textFilter
                Else
                    context.FirstFilterControl = New TextChoiceFilterControl(firstFilter, columnName) With {.DataContext = firstFilter}
                End If
                context.SecondFilterControl = Nothing
            End If

            Me.Owner.CommandService.ExecuteDefaultCommand(CommandId.FilterButtonTap, context)
        End Sub

    End Class

End Namespace