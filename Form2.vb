﻿Imports OfficeOpenXml
Imports System.IO
Imports System.Configuration

Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadExcelData()
    End Sub

    Private Sub LoadExcelData()
        Dim folderPath As String = ConfigurationManager.AppSettings("DefaultPath")
        Dim fileName As String = Path.Combine(folderPath, DateTime.Now.ToString("yyyy-MM-dd") & ".xlsx")
        Dim fileInfo As New FileInfo(fileName)
        If Not fileInfo.Exists Then
            MessageBox.Show("No data available for today.")
            Return
        End If
        Using package As New ExcelPackage(fileInfo)
            Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets(1)
            Dim startRow As Integer = 2
            For row As Integer = startRow To worksheet.Dimension.End.Row
                Dim sNo As String = worksheet.Cells(row, 1).Text
                Dim alarmName As String = worksheet.Cells(row, 2).Text
                Dim alarmCode As String = worksheet.Cells(row, 3).Text
                Dim time As String = worksheet.Cells(row, 4).Text
                Dim recipe As String = worksheet.Cells(row, 5).Text
                Dim alarmNameCode As String = $"{alarmName} - {alarmCode}"
                Guna2DataGridView1.Rows.Add(sNo, alarmNameCode, time, recipe)
            Next
        End Using
    End Sub
End Class
