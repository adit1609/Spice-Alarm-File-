Imports ActUtlTypeLib

Public Class Warning
    Dim plc As New ActUtlType
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        EXCEL.Checkagain = 0
        plc.SetDevice("M100", 1)
        'plc.SetDevice("D222", 0)
        Me.Close()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Alarms_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        plc.ActLogicalStationNumber = 1
        plc.Open()
        EXCEL.Checkagain = 1
        Me.BringToFront()
    End Sub

    Private Sub Alarms_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        EXCEL.Checkagain = 0

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        plc.SetDevice("D222", 0)
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class