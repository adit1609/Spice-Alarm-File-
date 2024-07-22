Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports System.IO

Public Class Form6
    Private isLocked As Boolean = False
    Private lockedMousePosition As Point
    Private currentMousePosition As Point
    Private savedMousePosition As Point
    Private pythonProcess As Process

    Private Sub Label1_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.Click
        If isLocked Then
            isLocked = False
        Else
            isLocked = True
            lockedMousePosition = e.Location
        End If
    End Sub

    Public Sub RunPythonScript()
        Dim pythonPath As String = "C:/Users/adity/AppData/Local/Programs/Python/Python312/python.exe"
        Dim scriptPath As String = "A:\Program\5.py"
        Dim imagePath As String = String.Empty
        Dim resultImagePath As String = "A:\Program\output.png"

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
        Dim regex As New Regex("Detected center at: \((?<x>\d+), (?<y>\d+)\)")
        Dim match As Match = regex.Match(output)
        If match.Success Then
            Dim x As String = match.Groups("x").Value
            Dim y As String = match.Groups("y").Value

            ' Update UI with coordinates
            RichTextBox1.Text = $"Detected center at: ({x}, {y})"
        Else
            RichTextBox1.Text = "No coordinates found"
        End If

        Console.WriteLine("Output: " & output)
        Console.WriteLine("Error: " & [error])

        ' Dispose of the previous image if it exists
        If PictureBox1.Image IsNot Nothing Then
            PictureBox1.Image.Dispose()
            PictureBox1.Image = Nothing
        End If

        ' Display the resulting image in the picture box
        If File.Exists(resultImagePath) Then
            PictureBox1.Image = Image.FromFile(resultImagePath)
        Else
            MessageBox.Show("Error: Processed image not found.")
        End If
    End Sub

    Private Sub Guna2PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles Guna2PictureBox1.MouseMove
        If isLocked Then
            currentMousePosition = lockedMousePosition
        Else
            currentMousePosition = e.Location
        End If
        ' Label1.Text = "X: " & currentMousePosition.X
        ' Label2.Text = "Y: " & currentMousePosition.Y
        Guna2PictureBox1.Invalidate()
    End Sub

    Private squareSize As Integer = 50

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        squareSize = CInt(NumericUpDown1.Value)
        Guna2PictureBox1.Invalidate()
    End Sub

    Private Sub Guna2PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles Guna2PictureBox1.Paint
        If Guna2PictureBox1.ClientRectangle.Contains(currentMousePosition) Then
            Dim g As Graphics = e.Graphics
            Dim pen As New Pen(Color.Red, 1)

            ' Draw crosshair lines
            g.DrawLine(pen, 0, currentMousePosition.Y, Guna2PictureBox1.Width, currentMousePosition.Y)
            g.DrawLine(pen, currentMousePosition.X, 0, currentMousePosition.X, Guna2PictureBox1.Height)

            ' Draw fiducial mark (+ sign with square shape and circle)
            Dim fiducialPen As New Pen(Color.Red, 3)

            ' Calculate sizes for square and circle
            Dim circleSize As Integer = CInt(squareSize * 0.7)  ' Circle size is 70% of square size

            ' Ensure circle size is smaller than square size
            If circleSize >= squareSize Then
                circleSize = CInt(squareSize * 0.7)
            End If

            ' Calculate rectangles
            Dim squareRect As New Rectangle(currentMousePosition.X - squareSize / 2, currentMousePosition.Y - squareSize / 2, squareSize, squareSize)
            Dim circleRect As New Rectangle(currentMousePosition.X - circleSize / 2, currentMousePosition.Y - circleSize / 2, circleSize, circleSize)

            ' Draw square and circle with fiducialPen
            g.DrawRectangle(fiducialPen, squareRect)
            g.DrawEllipse(fiducialPen, circleRect)
        End If
    End Sub

    Private Sub Guna2Button1_Click(sender As Object, e As EventArgs) Handles Guna2Button1.Click
        If Guna2PictureBox1.Image IsNot Nothing Then
            Dim bitmap As New Bitmap(squareSize, squareSize) ' Create a new bitmap with the size of the square
            Dim graphics As Graphics = Graphics.FromImage(bitmap)

            ' Draw the image inside the PictureBox onto the new bitmap,
            ' but only the region inside the square
            graphics.DrawImage(Guna2PictureBox1.Image,
                           New Rectangle(0, 0, squareSize, squareSize),
                           New Rectangle(currentMousePosition.X - squareSize / 2, currentMousePosition.Y - squareSize / 2, squareSize, squareSize),
                           GraphicsUnit.Pixel)

            ' Save the new bitmap to the specified file path
            Dim filePath As String = "A:\Project\Visual Path\image.png"
            bitmap.Save(filePath, Imaging.ImageFormat.Png)
            MessageBox.Show("Image saved successfully!")

            ' Save the current crosshair position and square size
            savedMousePosition = currentMousePosition
            savedSquareSize = squareSize
        Else
            MessageBox.Show("No image to save!")
        End If

        If pythonProcess IsNot Nothing AndAlso Not pythonProcess.HasExited Then
            pythonProcess.Kill()
            pythonProcess.Dispose()
            pythonProcess = Nothing
        End If

        ' Reset PictureBox1
        If PictureBox1.Image IsNot Nothing Then
            PictureBox1.Image.Dispose()
            PictureBox1.Image = Nothing
        End If
    End Sub


    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NumericUpDown1.Minimum = 50
        NumericUpDown1.Maximum = 300

        ' Initialize savedSquareSize to a valid value
        savedSquareSize = CInt(NumericUpDown1.Value)

        ' Update squareSize based on NumericUpDown1 value
        squareSize = CInt(NumericUpDown1.Value)

        ' Trigger repaint of Guna2PictureBox1 to reflect changes
        Guna2PictureBox1.Invalidate()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RunPythonScript()
    End Sub
    Private savedSquareSize As Integer

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        currentMousePosition = savedMousePosition
        lockedMousePosition = savedMousePosition
        isLocked = True

        ' Restore squareSize to its saved value, ensuring it is within the valid range
        If savedSquareSize >= NumericUpDown1.Minimum AndAlso savedSquareSize <= NumericUpDown1.Maximum Then
            squareSize = savedSquareSize
            NumericUpDown1.Value = savedSquareSize ' Update the numeric up-down control
        Else
            MessageBox.Show("Saved square size is out of range. Please adjust manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        ' Trigger repaint of Guna2PictureBox1 to reflect changes
        Guna2PictureBox1.Invalidate()
    End Sub


    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged

    End Sub

    Private Sub Guna2PictureBox1_Click(sender As Object, e As EventArgs) Handles Guna2PictureBox1.Click

    End Sub
End Class
