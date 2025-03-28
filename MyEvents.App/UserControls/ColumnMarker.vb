﻿Namespace Global.MyEvents.App.UserControls

    Public Class ColumnMarker

        Public Shared Function GetRequiresCustomFiltering(obj As DependencyObject) As Boolean
            Return obj.GetValue(RequiresCustomFilteringProperty)
        End Function

        Public Shared Sub SetRequiresCustomFiltering(obj As DependencyObject, value As Boolean)
            obj.SetValue(RequiresCustomFilteringProperty, value)
        End Sub

        Public Shared ReadOnly Property RequiresCustomFilteringProperty As DependencyProperty =
           DependencyProperty.RegisterAttached("RequiresCustomFiltering",
                                            GetType(Boolean),
                                            GetType(ColumnMarker),
                                            New PropertyMetadata(False))

        Public Shared Function GetColumnName(obj As DependencyObject) As String
            Return obj.GetValue(ColumnNameProperty)
        End Function

        Public Shared Sub SetColumnName(obj As DependencyObject, value As String)
            obj.SetValue(ColumnNameProperty, value)
        End Sub

        Public Shared ReadOnly Property ColumnNameProperty As DependencyProperty =
           DependencyProperty.RegisterAttached("ColumnName",
                                            GetType(String),
                                            GetType(ColumnMarker),
                                            New PropertyMetadata(""))

        Public Shared Function GetIsTextFilter(obj As DependencyObject) As Boolean
            Return obj.GetValue(IsTextFilterProperty)
        End Function

        Public Shared Sub SetIsTextFilter(obj As DependencyObject, value As Boolean)
            obj.SetValue(IsTextFilterProperty, value)
        End Sub

        Public Shared ReadOnly Property IsTextFilterProperty As DependencyProperty =
           DependencyProperty.RegisterAttached("IsTextFilter",
                                            GetType(Boolean),
                                            GetType(ColumnMarker),
                                            New PropertyMetadata(False))

    End Class

End Namespace