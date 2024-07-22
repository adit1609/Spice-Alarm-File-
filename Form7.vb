Imports System.Diagnostics
Imports System.IO
Imports System.Text.RegularExpressions
Public Class Form7

    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click
        Guna2PictureBox1.Cursor = Cursors.Cross
    End Sub
    Public Sub RunPythonScript()
        Dim pythonPath As String = "C:/Users/adity/AppData/Local/Programs/Python/Python312/python.exe"
        Dim scriptPath As String = "A:\Project\Pyton\circle.py"
        Dim imagePath As String = "A:\Screenshot 2024-06-20 220943.png"
        Dim resultImagePath As String = "A:\Project\Pyton\detected_fiducials_watershed.png"

        Dim startInfo As New ProcessStartInfo(pythonPath)
        startInfo.Arguments = scriptPath & " " & imagePath
        startInfo.UseShellExecute = False
        startInfo.RedirectStandardOutput = True
        startInfo.RedirectStandardError = True

        Dim process As New Process()
        process.StartInfo = startInfo
        process.Start()

        Dim output As String = process.StandardOutput.ReadToEnd()
        Dim [error] As String = process.StandardError.ReadToEnd()

        process.WaitForExit()

        ' Extract the x and y coordinates from the output
        Dim regex As New Regex("Center point of circle \(in cm\): \((?<x>[\d\.]+), (?<y>[\d\.]+)\)")
        Dim match As Match = regex.Match(output)
        If match.Success Then
            Dim x As String = match.Groups("x").Value
            Dim y As String = match.Groups("y").Value

            RichTextBox1.Text = x
            RichTextBox2.Text = y
        Else
            RichTextBox1.Text = "No coordinates found"
            RichTextBox2.Text = "No coordinates found"
        End If

        Console.WriteLine("Output: " & output)
        Console.WriteLine("Error: " & [error])

        ' Display the resulting image in the picture box
        If File.Exists(resultImagePath) Then
            Guna2PictureBox1.Image = Image.FromFile(resultImagePath)
        Else
            MessageBox.Show("Error: Processed image not found.")
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RunPythonScript()
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged

    End Sub

    Private Sub RichTextBox2_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox2.TextChanged

    End Sub
End Class