Imports MyEvents.Models

Namespace Global.MyEvents.App.ValueConverters

    Public Class PerformanceTypeToStringConverter

        Implements IValueConverter

        ' <summary>
        ' Modifies the source data before passing it to the target for display in the UI.
        ' </summary>
        ' <param name="value">The source data being passed to the target.</param>
        ' <param name="targetType">The type of the target property, as a type reference (System.Type for Microsoft .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).</param>
        ' <param name="parameter">An optional parameter to be used in the converter logic.</param>
        ' <param name="language">The language of the conversion.</param>
        ' <returns>The value to be passed to the target dependency property.</returns>
        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            Dim result As String = ""
            If value IsNot Nothing Then
                Dim type As Performance.PerformanceType = DirectCast(value, Performance.PerformanceType)
                Select Case type
                    Case Performance.PerformanceType.KeepValue
                        result = App.Texts.GetString("KeepValue")
                    Case Performance.PerformanceType.Ballet
                        result = App.Texts.GetString("Ballet")
                    Case Performance.PerformanceType.Opera
                        result = App.Texts.GetString("Opera")
                    Case Performance.PerformanceType.Cinema
                        result = App.Texts.GetString("Cinema")
                    Case Performance.PerformanceType.Concert
                        result = App.Texts.GetString("Concert")
                    Case Performance.PerformanceType.OpenAir
                        result = App.Texts.GetString("OpenAir")
                    Case Performance.PerformanceType.Theater
                        result = App.Texts.GetString("Theater")
                    Case Performance.PerformanceType.Planned
                        result = App.Texts.GetString("Planned")
                End Select
            End If

            Return result
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Dim result As Performance.PerformanceType = Performance.PerformanceType.Undefined
            If value IsNot Nothing Then
                If TypeOf value Is Performance.PerformanceType Then
                    Return value
                Else
                    Dim type As String = DirectCast(value, String)
                    If type.Equals(App.Texts.GetString("Ballet")) Then
                        result = Performance.PerformanceType.Ballet
                    ElseIf type.Equals(App.Texts.GetString("Opera")) Then
                        result = Performance.PerformanceType.Opera
                    ElseIf type.Equals(App.Texts.GetString("Cinema")) Then
                        result = Performance.PerformanceType.Cinema
                    ElseIf type.Equals(App.Texts.GetString("Concert")) Then
                        result = Performance.PerformanceType.Concert
                    ElseIf type.Equals(App.Texts.GetString("OpenAir")) Then
                        result = Performance.PerformanceType.OpenAir
                    ElseIf type.Equals(App.Texts.GetString("Theater")) Then
                        result = Performance.PerformanceType.Theater
                    ElseIf type.Equals(App.Texts.GetString("Planned")) Then
                        result = Performance.PerformanceType.Planned
                    End If
                End If
            End If
            Return result
        End Function
    End Class

End Namespace

