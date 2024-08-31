Imports ActUtlTypeLib
Imports OfficeOpenXml
Imports System.IO
Imports System.Configuration
Imports System.ComponentModel
Public Class EXCEL
    Dim plc As New ActUtlType
    Dim dval As Integer
    Dim prevDval As Integer = 0
    Public Checkagain As Integer = 0
    Private alarmForm As Warning = Nothing
    Private convertedValue As Single

    Private Sub EXCEL_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        plc.ActLogicalStationNumber = 1
        plc.Open()
        Alarm.InitializeAlarms()
        'Timer2.Start()
        'Timer1.Start()
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Label1.Text = DateTime.Now.ToString("dd MMM HH:mm:ss")
        plc.GetDevice("D222", dval)
        'here i have to watch the pervious value 
        If dval <> prevDval Then
            prevDval = dval 'values need to change and check here /// wednesday checked 
            If dval <> 0 Then
                DisplayAlarmAsync(dval)
            End If
        ElseIf dval = prevDval AndAlso Checkagain = 0 Then
            ' display the alarm again if you find the value is not increased or not changing even the alarm is not changed after clearing also 
            If dval <> 0 Then
                DisplayAlarmAsync(dval)
            End If
        End If
    End Sub

    Private Async Sub DisplayAlarmAsync(currentDval As Integer)
        'form to be opened 
        If alarmForm Is Nothing OrElse alarmForm.IsDisposed Then

            Dim alarm As Alarm = Await Task.Run(Function()
                                                    Return Alarm.Alarms.FirstOrDefault(Function(a) a.Number = currentDval)
                                                End Function)

            ' show on the main thread unltil it is cleared 
            Me.Invoke(Sub()
                          If alarmForm IsNot Nothing AndAlso Not alarmForm.IsDisposed Then
                              ' If the form is already open, just update the labels
                              alarmForm.Label2.Text = currentDval.ToString()
                              alarmForm.Label3.Text = If(alarm IsNot Nothing, alarm.Name, "Unknown Alarm Code!")
                              alarmForm.TopMost = True
                          Else
                              ' Otherwise, create a new instance of the form
                              alarmForm = New Warning()
                              alarmForm.Label2.Text = currentDval.ToString()
                              alarmForm.Label3.Text = If(alarm IsNot Nothing, alarm.Name, "Unknown Alarm Code!")
                              alarmForm.StartPosition = FormStartPosition.CenterParent
                              alarmForm.TopMost = True
                              ' Show the form, centered in ExcelForm
                              alarmForm.BringToFront()
                              alarmForm.Show()

                          End If
                          SaveAlarmToExcel(alarmForm.Label3.Text, alarmForm.Label2.Text, Label1.Text, Label2.Text, Label3.Text)
                      End Sub)
        End If
    End Sub
    Private Sub SaveAlarmToExcel(alarmName As String, alarmCode As String, time As String, recipe As String, user As String)
        ' Retrieve the default path from app.config
        Dim folderPath As String = ConfigurationManager.AppSettings("DefaultPath")

        ' Ensure folderPath is not null or empty
        If String.IsNullOrEmpty(folderPath) Then
            MessageBox.Show("The folder path is not set in the configuration file.")
            Return
        End If

        ' Create the directory if it doesn't exist
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If

        ' Define the Excel file path with the current date as the filename
        Dim fileName As String = Path.Combine(folderPath, DateTime.Now.ToString("yyyy-MM-dd") & ".xlsx")
        Dim fileInfo As New FileInfo(fileName)

        ' Create or open the Excel file
        Using package As New ExcelPackage(fileInfo)
            Dim worksheet As ExcelWorksheet

            ' Check if any worksheets exist
            If package.Workbook.Worksheets.Count = 0 Then
                ' Add a new worksheet if none exists
                worksheet = package.Workbook.Worksheets.Add("Alarms")
                ' Add headers to the worksheet
                worksheet.Cells("A1").Value = "S.No"
                worksheet.Cells("B1").Value = "Alarm Name"
                worksheet.Cells("C1").Value = "Alarm Code"
                worksheet.Cells("D1").Value = "Time"
                worksheet.Cells("E1").Value = "Recipe"
                worksheet.Cells("F1").Value = "User"
                worksheet.Cells("G1").Value = "REMEDY"
                worksheet.Cells("H1").Value = "CATEGORY"
                worksheet.Cells("I1").Value = "ALARM DURATION"
            Else
                ' Access the first worksheet
                worksheet = package.Workbook.Worksheets(1)
            End If

            ' Find the next available row
            Dim nextRow As Integer
            If worksheet.Dimension IsNot Nothing Then
                nextRow = worksheet.Dimension.End.Row + 1
            Else
                nextRow = 2 ' Start from row 2 if no data exists
            End If

            ' Write the data to the worksheet
            worksheet.Cells(nextRow, 1).Value = nextRow - 1 ' S.No
            worksheet.Cells(nextRow, 2).Value = alarmName
            worksheet.Cells(nextRow, 3).Value = alarmCode
            worksheet.Cells(nextRow, 4).Value = time
            worksheet.Cells(nextRow, 5).Value = recipe
            worksheet.Cells(nextRow, 6).Value = user

            ' Save the changes to the Excel file
            package.Save()
        End Using
    End Sub
    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        Form2.Show()
    End Sub
    Public dec As Integer
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        'plc.GetDevice("D202", dec)

        Dim X(1) As Integer
        plc.GetDevice("D202", X(0))
        plc.GetDevice("D203", X(1))

        convertedValue = ConvertWordToFloat(X)
        e.Result = convertedValue

    End Sub


    Private Sub BackgroundWorker2_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker2.DoWork
        plc.SetDevice("M202", 0)
    End Sub

    'Private Sub Button1_MouseDown(sender As Object, e As MouseEventArgs) Handles Button1.MouseDown
    '    BackgroundWorker1.RunWorkerAsync()
    'End Sub
    'Private Sub Button1_MouseUp(sender As Object, e As MouseEventArgs) Handles Button1.MouseUp
    '    BackgroundWorker2.RunWorkerAsync()
    'End Sub

    'Private Sub Button2_MouseDown(sender As Object, e As MouseEventArgs) Handles Button2.MouseDown
    '    plc.SetDevice("M202", 1)
    'End Sub

    'Private Sub Button2_MouseUp(sender As Object, e As MouseEventArgs) Handles Button2.MouseUp
    '    plc.SetDevice("M202", 0)
    'End Sub
    Private Async Function SetDeviceStateAsync(state As Integer) As Task
        Await Task.Run(Sub() plc.SetDevice("M202", state))
    End Function

    Private Async Sub Button1_MouseDown(sender As Object, e As MouseEventArgs) Handles Button1.MouseDown
        Await SetDeviceStateAsync(1)

    End Sub

    Private Async Sub Button1_MouseUp(sender As Object, e As MouseEventArgs) Handles Button1.MouseUp
        Await SetDeviceStateAsync(0)
    End Sub

    Private Sub Guna2TextBox1_TextChanged(sender As Object, e As EventArgs) Handles Guna2TextBox1.TextChanged

    End Sub

    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For i As Integer = 0 To 100
            Await SetDevice(i)
        Next

    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Guna2TextBox1.Text = dec.ToString()
        Guna2TextBox2.Text = e.Result.ToString()

    End Sub

    Private Sub Timer1_Tick_1(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Not BackgroundWorker1.IsBusy Then
            BackgroundWorker1.RunWorkerAsync()
        End If
    End Sub
    Private Async Function SetDevice(value As Integer) As Task
        Await Task.Run(Sub() plc.SetDevice("D202", value))
    End Function
    Public Function ConvertWordToFloat(ByVal register As Integer()) As Single
        Dim bytes(3) As Byte
        Dim lowWordBytes() As Byte = BitConverter.GetBytes(register(0))
        Dim highWordBytes() As Byte = BitConverter.GetBytes(register(1))

        Array.Copy(lowWordBytes, 0, bytes, 0, 2)
        Array.Copy(highWordBytes, 0, bytes, 2, 2)

        Return BitConverter.ToSingle(bytes, 0)
    End Function

End Class
