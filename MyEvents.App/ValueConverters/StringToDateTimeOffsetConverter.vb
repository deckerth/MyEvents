﻿Imports System.Text.RegularExpressions
Imports Windows.Globalization.DateTimeFormatting

Namespace Global.MyEvents.App.ValueConverters

    Public Class StringToDateTimeOffsetConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            ' Convert String to System.Nullable(Of DateTimeOffset)

            If value Is Nothing Then
                Return Nothing
            Else
                Dim result As DateTime
                If DateTime.TryParse(value, result) Then
                    Dim dto As DateTimeOffset = result
                    Return dto
                Else
                    Return DateTimeOffset.Now
                End If
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Dim result As String = ""

            If value IsNot Nothing Then
                Dim dto As DateTimeOffset = DirectCast(value, DateTimeOffset)
                result = DateTimeFormatter.ShortDate.Format(dto)
                ' Remove LEFT-TO-RIGHT character (0xe2808e)
                result = Regex.Replace(result, "[^\u0009\u000A\u000D\u0020-\u007E]", "")
            End If

            Return result
        End Function

    End Class

End Namespace
