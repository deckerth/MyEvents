Namespace Global.MyEvents.App.ValueConverters

    Public Class DoubleToStringConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            If value Is Nothing Then
                Return ""
            Else
                Return DirectCast(value, Double).ToString
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Dim result As Double = 0
            If value IsNot Nothing Then
                Double.TryParse(DirectCast(value, String), result)
            End If
            Return result
        End Function
    End Class

End Namespace
